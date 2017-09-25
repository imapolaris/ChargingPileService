using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class ChargingController : OperatorBase
    {
        /// <summary>
        /// 开始充电
        /// </summary>
        /// <param name="serialNumber">充电桩序列号</param>
        /// <returns></returns>
        public IHttpActionResult StartCharging(string serialNumber)
        {
            return Ok(true);
        }
    }
}
