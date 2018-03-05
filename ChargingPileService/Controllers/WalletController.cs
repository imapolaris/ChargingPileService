using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using ChargingPileService.Common;
using ChargingPileService.Models;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.History;
using Soaring.WebMonter.Contract.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WxPayAPI;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/wallet")]
    public class WalletController : OperatorBase
    {
        [HttpGet]
        [Route("balance")]
        public IHttpActionResult GetBalance(string userId)
        {
            try
            {
                var theWallet = SysDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                return Ok(Models.SingleResult<double>.Succeed("查询成功！", theWallet.Remaining));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("records")]
        public IEnumerable<PayRecord> GetRechargeRecords(string userId)
        {
            return HisDbContext.PayRecords.Where(_=>_.CustomerId == userId && _.IsValid == true);
        }

        [HttpDelete]
        [Route("clear")]
        public IHttpActionResult ClearRechargeRecords(dynamic obj)
        {
            try
            {
                string userId = obj.userId;

                var result = HisDbContext.PayRecords.Where(_ => _.CustomerId == userId).ToList();
                if (result != null && result.Count() > 0)
                {
                    result.ForEach(_ => _.IsValid = false);
                    HisDbContext.SaveChanges();
                }
                return Ok(SimpleResult.Succeed("操作完成！"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("操作失败！"));
            }
        }

        [HttpPost]
        [Route("recharge")]
        public IHttpActionResult MakeOneRecharge(dynamic obj)
        {
            try
            {
                string userId = obj.userId;
                double money = obj.money;
                string payway = obj.payway;

                var theWallet = SysDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                if (theWallet == null)
                {
                    SysDbContext.Wallets.Add(new Sys_Wallet()
                    {
                        CustomerId = userId,
                        Remaining = money,
                    });
                }
                else
                {
                    theWallet.Remaining += money;
                }

                SysDbContext.SaveChanges();

                HisDbContext.PayRecords.Add(new PayRecord()
                {
                    PayDate = DateTime.Now,
                    PayWay = payway,
                    PayMoney = money,
                    CustomerId = userId,
                });
                
                HisDbContext.SaveChanges();

                var returnVal = new Models.SingleResult<double>(true, "充值成功！", theWallet.Remaining);
                return Ok(returnVal);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("充值失败！"));
        }

        [HttpPost]
        [Route("alipay")]
        public IHttpActionResult Alipay(dynamic obj)
        {
            try
            {
                double money = obj.money;

                IAopClient client = new DefaultAopClient(AliPayConfig.ServerUrl, AliPayConfig.APPID, AliPayConfig.APP_PRIVATE_KEY,
                    "json", "1.0", "RSA2", AliPayConfig.ALIPAY_PUBLIC_KEY, AliPayConfig.CHARSET, false);
                AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();

                AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
                model.Subject = ProductDesc;
                model.Body = ProductDesc + "-支付宝支付";
                model.TotalAmount = money.ToString();
                model.ProductCode = "QUICK_MSECURITY_PAY";
                model.OutTradeNo = Guid.NewGuid().ToString();
                model.TimeoutExpress = "10m";
                //model.SellerId = "";

                request.SetBizModel(model);
                //request.SetNotifyUrl("");

                AlipayTradeAppPayResponse response = client.SdkExecute(request);

                return Ok(Models.SingleResult<string>.Succeed("订单预生成成功！", response.Body));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("充值失败！"));
            }
        }


        private static readonly string ProductDesc = ConfigHelper.ProductDesc;

        [HttpPost]
        [Route("wxpay")]
        public IHttpActionResult Wxpay(dynamic obj)
        {
            try
            {
                double money = obj.money;
                money *= 100;
                money = 1;

                //统一下单
                WxPayData data = new WxPayData();
                data.SetValue("body", ProductDesc);//商品描述
                data.SetValue("attach", ProductDesc + "-微信支付");//附加数据
                data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
                data.SetValue("total_fee", money.ToString());//总金额（单位为【分】）
                data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
                data.SetValue("goods_tag", "wx-cp");//商品标记
                data.SetValue("trade_type", "APP");//交易类型

                WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
                var returnCode = result.GetValue("return_code").ToString();
                if (returnCode == "FAIL")
                {
                    return Ok(SimpleResult.Failed("充值失败！"));
                }

                WxPayData payObject = new WxPayData();
                payObject.SetValue("appid", Common.WxPayConfig.APPID);
                payObject.SetValue("partnerid", Common.WxPayConfig.MCHID);
                payObject.SetValue("prepayid", result.GetValue("prepay_id"));
                payObject.SetValue("package", "Sign=WXPay");
                payObject.SetValue("noncestr", WxPayApi.GenerateNonceStr());
                payObject.SetValue("timestamp", DateTime.Now.ConvertToTimeStampX().ToString());
                payObject.SetValue("sign", payObject.MakeSign());

                return Ok(Models.SingleResult<string>.Succeed("预支付完成！", payObject.ToJson()));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("充值失败！"));
        }
    }
}
