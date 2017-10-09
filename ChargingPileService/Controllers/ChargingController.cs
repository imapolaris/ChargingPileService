using ChargingPileService.Models;
using CPS.Communication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/charging")]
    public class ChargingController : OperatorBase
    {
        IChargingService chargingService = new ChargingService();

        /// <summary>
        /// 开始充电
        /// </summary>
        /// <param name="serialNumber">充电桩序列号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("start/{sn}")]
        public IHttpActionResult StartCharging(string sn)
        {
            var callback = new Func<string, IHttpActionResult>((serialNumber) =>
             {
                 var status = chargingService.startCharging(serialNumber);
                 if (status)
                     return Ok(SimpleResult.Succeed("请开始充电！"));
                 else
                     return Ok(SimpleResult.Failed("请求服务失败！"));
             });
            return ValidSerialNumber(sn, callback);
        }

        /// <summary>
        /// 结束充电
        /// </summary>
        /// <param name="serialNumber">充电桩序列号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("stop/{sn}")]
        public IHttpActionResult StopCharging(string sn)
        {
            var callback = new Func<string, IHttpActionResult>((serialNumber) =>
            {
                var status = chargingService.stopCharging(serialNumber);
                if (status)
                    return Ok(SimpleResult.Succeed("已结束充电！"));
                else
                    return Ok(SimpleResult.Failed("请求服务失败！"));
            });
            return ValidSerialNumber(sn, callback);
        }

        /// <summary>
        /// 请求充电状态
        /// </summary>
        /// <param name="serialNumber">充电桩序列号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("status/{sn}")]
        public IHttpActionResult RequestChargingStatus(string sn)
        {
            var callback = new Func<string, IHttpActionResult>((serialNumber) =>
            {
                var info = chargingService.getChargingStatus(serialNumber);
                var returnVal = new Models.SingleResult<object>(true, "请求成功！", info);
                return Ok(returnVal);
            });
            return ValidSerialNumber(sn, callback);
        }

        [NonAction]
        private IHttpActionResult ValidSerialNumber(string serialNumber, Func<string, IHttpActionResult> callback)
        {
            var exists = EntityContext.CPS_ChargingPile.Any(_ => _.SerialNumber == serialNumber);

            // for test.
            exists = true;

            if (exists)
            {
                if (callback != null)
                {
                    return callback.Invoke(serialNumber);
                }
            }

            return Ok(SimpleResult.Failed("编号不存在！"));
        }
    }
}
