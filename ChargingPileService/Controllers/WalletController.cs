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
                var remaining = 0.0;
                if (theWallet != null)
                    remaining = theWallet.Remaining;
                return Ok(Models.SingleResult<double>.Succeed("查询成功！", remaining));
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
            return HisDbContext.PayRecords.Where(_=>_.CustomerId == userId && _.IsValid == true).OrderByDescending(_=>_.PayDate);
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
                string tradeno = obj.tradeno;

                // 检查账单是否已存在（支付结果会异步通知）
                var record = HisDbContext.PayRecords.Where(_ => _.DealNo == tradeno).FirstOrDefault();
                if (record != null)
                {
                    var theWallet = SysDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                    return Ok(new Models.SingleResult<double>(true, "充值成功！", theWallet.Remaining));
                }
                else
                {
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

                    int result = SysDbContext.SaveChanges();
                    if (result > 0)
                    {
                        // 添加充值账单记录
                        HisDbContext.PayRecords.Add(new PayRecord()
                        {
                            PayDate = DateTime.Now,
                            PayWay = payway,
                            PayMoney = money,
                            CustomerId = userId,
                            DealNo = tradeno,
                        });

                        HisDbContext.SaveChanges();

                        var returnVal = new Models.SingleResult<double>(true, "充值成功！", theWallet.Remaining);
                        return Ok(returnVal);
                    }
                }
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
                string userId = obj.userId;

                IAopClient client = new DefaultAopClient(AliPayConfig.ServerUrl, AliPayConfig.APPID, AliPayConfig.APP_PRIVATE_KEY,
                    "json", "1.0", AliPayConfig.SIGNTYPE, AliPayConfig.ALIPAY_PUBLIC_KEY, AliPayConfig.CHARSET, false);
                AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();

                var outtradeno = Guid.NewGuid().ToString();
                AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
                model.Subject = ProductDesc;
                model.Body = ProductDesc + "-支付宝支付";
                model.TotalAmount = money.ToString();
                model.ProductCode = "QUICK_MSECURITY_PAY";
                model.OutTradeNo = outtradeno;
                model.TimeoutExpress = "10m";
                model.PassbackParams = userId;

                //model.SellerId = "";

                request.SetBizModel(model);
                request.SetNotifyUrl(AliPayConfig.NOTIFY_URL);

                AlipayTradeAppPayResponse response = client.SdkExecute(request);
                response.OutTradeNo = outtradeno;

                return Ok(Models.SingleResult<AlipayTradeAppPayResponse>.Succeed("订单预生成成功！", response));
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
                string userId = obj.userId;
                double money = obj.money; // 单位分
                money *= 100;

                var trade_no = WxPayApi.GenerateOutTradeNo();
                //统一下单
                WxPayData data = new WxPayData();
                data.SetValue("body", ProductDesc);//商品描述
                data.SetValue("attach", ProductDesc + "-微信支付");//附加数据
                data.SetValue("out_trade_no", trade_no);//随机字符串
                data.SetValue("total_fee", money.ToString());//总金额（单位为【分】）
                data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
                data.SetValue("goods_tag", "wx-cp");//商品标记
                data.SetValue("trade_type", "APP");//交易类型

                data.SetValue("notify_url", Common.WxPayConfig.NOTIFY_URL); //异步通知url
                data.SetValue("attach", userId);//附加数据，在查询API和支付通知中原样返回

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
                payObject.SetValue("tradeno", trade_no); // 商家订单号，对账单进行追踪

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
