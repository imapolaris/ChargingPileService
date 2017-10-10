using ChargingPileService.Models;
using CPS.Entities;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/wallet")]
    public class WalletController : OperatorBase
    {
        [HttpGet]
        [Route("balance/{userId}")]
        public IHttpActionResult Get(string userId)
        {
            try
            {
                var theWallet = EntityContext.CPS_Wallet.Where(_ => _.UserId == userId).First();

                var returnVal = new Models.SingleResult<double>(true, "", theWallet.Remaining);
                return Ok(returnVal);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return Ok(SimpleResult.Failed("没有发现余额记录..."));
        }

        [HttpGet]
        [Route("charge")]
        public IHttpActionResult MakeOneCharge(string userId, double money, string payway)
        {
            try
            {
                var theWallet = EntityContext.CPS_Wallet.Where(_ => _.UserId == userId).First();
                theWallet.Remaining += money;

                EntityContext.CPS_PayRecord.Add(new PayRecord()
                {
                    PayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PayWay = payway,
                    PayMoney = money,
                    UserId = userId,
                });

                EntityContext.SaveChanges();

                var returnVal = new Models.SingleResult<double>(true, "", theWallet.Remaining);
                return Ok(returnVal);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return Ok(SimpleResult.Failed("充值失败..."));
        }
    }
}
