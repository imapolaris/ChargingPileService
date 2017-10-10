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
    public class ChargingPilesController : OperatorBase
    {
        public IEnumerable<ChargingPile> GetChargingPilesByStationId(string id)
        {
            return EntityContext.CPS_ChargingPile.Where(_ => _.StationId == id);
        }

        [HttpGet]
        [Route("subscribe/{sn}")]
        public IHttpActionResult Subscribe(string sn)
        {
            var exists = EntityContext.CPS_ChargingPile.Any(_ => _.SerialNumber == sn);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("编号不存在！"));
            }

            try
            {
                var theCPile = EntityContext.CPS_ChargingPile.Where(_ => _.SerialNumber == sn).First();
                if (theCPile.Status != "在线")
                {
                    return Ok(SimpleResult.Failed("无法预约！"));
                }
                else
                {
                    theCPile.Status = "预约";
                    EntityContext.SaveChanges();
                }

                return Ok(SimpleResult.Succeed("预约成功！"));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }

            return Ok(SimpleResult.Failed("预约失败！"));
        }
    }
}
