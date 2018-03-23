using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChargingPileService.Common
{
    using WxPayAPI;
    using Soaring.WebMonter.DB;
    using Soaring.WebMonter.Contract;
    using System.Linq;
    using CPS.Infrastructure.Utils;
    using Soaring.WebMonter.Contract.Manager;
    using Soaring.WebMonter.Contract.History;

    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class WxPayResultNotify:Notify
    {
        public WxPayResultNotify(Page page):base(page)
        {
        }

        public override void ProcessNotify()
        {
            Logger.Info("收到微信支付结果异步通知");

            WxPayData notifyData = GetNotifyData();

            Log.Info(this.GetType().ToString(), "微信支付通知信息："+notifyData.ToJson());

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            //查询订单成功
            else
            {
                // 充电桩系统充值业务处理
                var tradeno = notifyData.GetValue("out_trade_no")?.ToString();
                if (!string.IsNullOrEmpty(tradeno))
                {
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "支付结果中商户订单号不存在");
                    Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                    page.Response.Write(res.ToXml());
                    page.Response.End();
                }

                var hisDbContext = new HistoryDbContext();
                var sysDbContext = new SystemDbContext();
                var record = hisDbContext.PayRecords.Where(_=>_.DealNo == tradeno).FirstOrDefault();
                if (record == null) // 账单不存在
                {
                    try
                    {
                        var userId = notifyData.GetValue("attach")?.ToString();
                        var money = double.Parse(notifyData.GetValue("total_fee").ToString());
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
                                PayDate = DateTime.Now,
                                PayWay = "微信支付",
                                PayMoney = money,
                                CustomerId = userId,
                                DealNo = tradeno,
                            });

                            hisDbContext.SaveChanges();

                            WxPayData resu = new WxPayData();
                            resu.SetValue("return_code", "SUCCESS");
                            resu.SetValue("return_msg", "OK");
                            Log.Info(this.GetType().ToString(), "order succeed : " + resu.ToXml());
                            page.Response.Write(resu.ToXml());
                            page.Response.End();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "业务逻辑处理出错，请求重新发送支付结果通知！");
                    Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                    page.Response.Write(res.ToXml());
                    page.Response.End();
                }
                else
                {
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    Log.Info(this.GetType().ToString(), "order had finished : " + res.ToXml());
                    page.Response.Write(res.ToXml());
                    page.Response.End();
                }
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
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