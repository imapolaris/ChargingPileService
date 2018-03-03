using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using ChargingPileService.Models;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.History;
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
        [Route("charge")]
        public IHttpActionResult MakeOneCharge(string userId, double money, string payway)
        {
            try
            {
                var theWallet = EntityContext.CPS_Wallet.Where(_ => _.UserId == userId).First();
                theWallet.Remaining += money;

                HisDbContext.PayRecords.Add(new PayRecord()
                {
                    PayDate = DateTime.Now,//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PayWay = payway,
                    PayMoney = money,
                    CustomerId = userId,
                });

                EntityContext.SaveChanges();

                var returnVal = new Models.SingleResult<double>(true, "", theWallet.Remaining);
                return Ok(returnVal);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return Ok(SimpleResult.Failed("充值失败..."));
        }

        private const string ALI_ServerUrl = @"https://openapi.alipaydev.com/gateway.do";
        private const string ALI_APPID = @"2016081600258765";
        private const string ALI_APP_PRIVATE_KEY = @"MIIEowIBAAKCAQEAye3hyJqoJWUsVn7su0ndEhVuxlMHAX5RyCdOoLoGF4YKZ3NIS2mUVNoPNk/sl+Q0XiLIZCPrRsA3l4QS4mwOkyJJvfEY5/3DV2TCuPzFnqSahxd3tAg5VeKEBFyA9TrkEy4SGifYJFU2Smx+x400I8EzxGY8FJG6UC4k4AIZShfvDv8ufgK1R+Ub81JsTnx3ogtwpi3/7s7DKm+Mm1JCkt/gEijetmIflJMG6YviQU6XLTcM4xkrEvDttcQ9Z/TNxw3JAgVgUsTmT1dWjhqEuxtWpCvW+JfQu+sa1lZ+MD9dfyPqaG99X0DlQBsMKYxKqQAinhx+eXHGDrfUbGWBoQIDAQABAoIBAFvYkHb/KXYA477f8mtpuF0OVJlukGQ0gZxJjLD8i+LNPBQ70mlCt440tPCeP94ClXMv3Pf3gn9m1KJdF33XanWwBdyYOhzjRqOMmCkuB/EVq5fAq9i+WN3gru2Q6bMhOzYiIWe2MdCs1YnaeXvolQuiSBqP6cntUtI/etRNABW7qFDaQBPhXPHENYKyDTjRE93jpHZbvO0dutfgH1H2fn1Mi0Z41g0WtKF7fRFzd82+jAJAgVLx5tc8swLVfVhd7ZlRmoVQsc7zRHjTrnpb/ry9TemkoKmIb+ZIiEFsF+0GRwqSzfybuUpzufmmLX9opvNVSB8LQbMGKRzA18i7KkUCgYEA5Uvx/i7kZ4XjDzyDj4xton+KyHCUvv8VTaWbsmYrfzXZGyJ5EIQunBz9/oM5Dq+s1Mt9o4GWtqtrvlWWM6VST+zgvWvZr0mVL+gS61npHL1h9qekNUIJQN6fvxg8IqktVhSLrXwOyAnqKkiyBO4evgz/+GJfsnp17sRBYZho3+cCgYEA4XIHAYU8eZTTPhz3WNAfXgCO8cqbhLkMK6giFmRET4NNi/aZYK60Ja0EBzKpfGGul4d/njOumVKywm6ldiI1VppZHsx2VDGnBalMlVYYKkwQR67NvdiXHtk+5rA69pVIJmlrYMQjB7VCFMJrwgrGizFGg5jua6ulm4t5bIlmgTcCgYBEBNhfb6efsg7eKTRZs+2d47nWpdbqJZ87LmJWdIp4rQ+fRgWlyaBN/Se3hVO6sJBTe53kj/+WZpmKl7b70RHu1bUBW+nyXqCb2nsqR7yoIuHZmndSuSknjiLTPCwyl/7z5xpcN03nN1G4g2ITplOGSzLvircaqcssLhm7CswphwKBgDnTb6yaKjrdS5nBAEjNiV4pMoEegOl4NYD1LVkk+siSW0+tPwYniZmoWUInYoW+4HOJk9hWVVCKf8OTceltONUv3fAiba+G1NqE5FnhrW0b+YkJc0hgx9Jn0tSFG3qoK8t+esZlSL7vZTXB8LXi4a5OQ1H55h3D90SAb/LBA4PdAoGBALUWv7YJZQ3NWo3T1/PpqvdrjJA3/SCZpO6QSrqHuoWDPCyADKaMJyBJCVi3/fe+UHYp5Zt0z9TeM3nJte0meHFoBGx4AZpwZ0hKC0J3DlUT4YCH61UEl6oZVmew6dM+TBCdkSO9RaLrpymH2w8wGWqpH30ilEU0v1kUWRn0JV4I";
        private const string ALI_ALIPAY_PUBLIC_KEY = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvStlrbYUILe6QVQNpZvQm2AIrLad92HCRawDrQT6lVSyssKE4jYep10vJeyOG9U2dQLVD/0ZANnIzeeER2RrFmOIEOcwCucRQGtgI7j/SCGlo2YT9MIUWWngx913cDtqvvE7DiMIOY9miKdjwNViSjVkVrSfv4oQxQ07ZKkNYGvwxglgtGnOhoBMTRmeF3qqKu+SY5Atip3M5VAaoLs6Rq6dlFaKyeVlhxaksHQvsfDNAsMA1uPZbGimg/sUhApJZkQ4QSjFz9XMChyCzxDMvVZYXWtaxaRqCOxZ6FYsL4F7Hdyw/51hLQIOcasABJ7jSCEUhRuE1/z4/20g5OQQSQIDAQAB";
        private const string CHARSET = @"utf-8";
        private static string temp = @"20170216test";

        [HttpGet]
        [Route("alipay")]
        public IHttpActionResult Alipay()
        {
            IAopClient client = new DefaultAopClient(ALI_ServerUrl, ALI_APPID, ALI_APP_PRIVATE_KEY, "json", "1.0", "RSA2", ALI_ALIPAY_PUBLIC_KEY, CHARSET, false);
            AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();

            AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            model.Body = "im test";
            model.Subject = "App pay test DoNet";
            model.TotalAmount = "0.01";
            model.ProductCode = "QUICK_MSECURITY_PAY";

            // for test.
            temp = temp + "1";
            model.OutTradeNo = temp;
            
            model.TimeoutExpress = "30m";
            //model.SellerId = "";

            request.SetBizModel(model);
            request.SetNotifyUrl("");

            AlipayTradeAppPayResponse response = client.SdkExecute(request);

            return Ok(response.Body);
        }

        [HttpGet]
        [Route("wxpay")]
        public IHttpActionResult Wxpay()
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", "test");//商品描述
            data.SetValue("attach", "test");//附加数据
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            data.SetValue("total_fee", 1);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", "jjj");//商品标记
            data.SetValue("trade_type", "APP");//交易类型

            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口

            WxPayData payObject = new WxPayData();
            payObject.SetValue("appid", WxPayConfig.APPID);
            payObject.SetValue("partnerid", WxPayConfig.MCHID);
            payObject.SetValue("prepayid", result.GetValue("prepay_id"));
            payObject.SetValue("package", "Sign=WXPay");
            payObject.SetValue("noncestr", WxPayApi.GenerateNonceStr());
            payObject.SetValue("timestamp", DateTime.Now.ConvertToTimeStampX().ToString());
            payObject.SetValue("sign", payObject.MakeSign());

            return Ok(payObject.ToJson());
        }
    }
}
