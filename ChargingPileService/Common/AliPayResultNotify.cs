using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChargingPileService.Common
{
    using Soaring.WebMonter.DB;
    using Soaring.WebMonter.Contract;
    using System.Linq;
    using CPS.Infrastructure.Utils;
    using Soaring.WebMonter.Contract.Manager;
    using Soaring.WebMonter.Contract.History;
    using System.Text;
    using AliPayAPI;
    using Aop.Api;
    using Aop.Api.Request;
    using Aop.Api.Domain;

    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class AliPayResultNotify
    {
        public Page page { get; set; }
        public AliPayResultNotify(Page page)
        {
            this.page = page;
        }

        /// <summary>
        /// 接收从支付宝后台发送过来的数据并验证签名
        /// </summary>
        /// <returns></returns>
        private AliPayData GetNotifyData()
        {
            // 接收从支付宝后台POST过来的数据
            System.IO.Stream s = page.Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            var postData = builder.ToString();
            Log.Info(this.GetType().ToString(), "Receive data from AliPay : " + builder.ToString());

            var decodedData = HttpUtility.UrlDecode(postData);
            Log.Info(this.GetType().ToString(), decodedData);

            // 数据验证签名
            AliPayData data = new AliPayData();
            try
            {
                data.FromUrl(postData);
            }
            catch (AliPayException ex)
            {
                Logger.Error(ex);
            }

            return data;
        }

        public void ProcessNotify()
        {
            Logger.Info("收到支付宝支付结果异步通知");

            var notifyData = GetNotifyData();

            var status = notifyData.GetValue("trade_status")?.ToString();
            if (string.IsNullOrEmpty(status) || (status != "TRADE_FINISHED" && status != "TRADE_SUCCESS"))
            {
                Log.Error(this.GetType().ToString(), "通知状态不是完成或成功");
                page.Response.End();
            }

            //检查支付结果中trade_no是否存在
            if (!notifyData.IsSet("trade_no"))
            {
                //若trade_no不存在，则立即返回结果给支付宝支付后台
                Log.Error(this.GetType().ToString(), "The Pay result is error : 支付结果中交易订单号不存在");
                page.Response.End();
            }

            string trade_no = notifyData.GetValue("trade_no")?.ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(trade_no))
            {
                //若订单查询失败，则立即返回结果给支付宝支付后台
                Log.Error(this.GetType().ToString(), "Order query failure : 订单查询失败");
                page.Response.End();
            }
            //查询订单成功
            else
            {
                // 充电桩系统充值业务处理
                var tradeno = notifyData.GetValue("out_trade_no")?.ToString();
                if (!string.IsNullOrEmpty(tradeno))
                {
                    Log.Error(this.GetType().ToString(), "The Pay result is error : 支付结果中商户订单号不存在");
                    page.Response.End();
                }

                var hisDbContext = new HistoryDbContext();
                var sysDbContext = new SystemDbContext();
                var record = hisDbContext.PayRecords.Where(_=>_.DealNo == tradeno).FirstOrDefault();
                if (record == null) // 账单不存在
                {
                    try
                    {
                        var userId = notifyData.GetValue("passback_params")?.ToString();
                        var money = double.Parse(notifyData.GetValue("total_amount").ToString());
                        // 修改钱包余额
                        var theWallet = sysDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                        if (theWallet == null)
                        {
                            sysDbContext.Wallets.Add(new Sys_Wallet()
                            {
                                CustomerId = userId,
                                Remaining = money,
                            });
                        }
                        else
                        {
                            theWallet.Remaining += money;
                        }

                        int result = sysDbContext.SaveChanges();
                        if (result > 0)
                        {
                            // 添加充值账单记录
                            hisDbContext.PayRecords.Add(new PayRecord()
                            {
                                CompanyCode = "CP_001",
                                PayDate = DateTime.Now,
                                PayWay = "支付宝支付",
                                PayMoney = money,
                                CustomerId = userId,
                                DealNo = tradeno,
                            });

                            hisDbContext.SaveChanges();

                            Log.Info(this.GetType().ToString(), $"{tradeno} completed :");
                            page.Response.Write("success");
                            page.Response.End();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    Log.Error(this.GetType().ToString(), "The Pay result is error : 业务逻辑处理出错，请求重新发送支付结果通知！");
                    page.Response.End();
                }
                else
                {
                    Log.Info(this.GetType().ToString(), "order had finished.");
                    page.Response.Write("success");
                    page.Response.End();
                }
            }
        }

        //查询订单
        private bool QueryOrder(string trade_no)
        {
            IAopClient client = new DefaultAopClient(AliPayConfig.ServerUrl, AliPayConfig.APPID, AliPayConfig.APP_PRIVATE_KEY,
                    "json", "1.0", AliPayConfig.SIGNTYPE, AliPayConfig.ALIPAY_PUBLIC_KEY, AliPayConfig.CHARSET, false);
            AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();
            AlipayTradeQueryModel model = new AlipayTradeQueryModel();
            model.TradeNo = trade_no;
            request.SetBizModel(model);
            var response = client.Execute(request);
            
            if (response.Code == "10000")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}