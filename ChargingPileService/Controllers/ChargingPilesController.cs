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

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/chargingpiles")]
    public class ChargingPilesController : OperatorBase
    {
        [HttpGet]
        public IEnumerable<Sys_ChargingPile> GetChargingPilesByStationId(string stationId)
        {
            // TODO: 后面改为从缓存中读取。
            return SysDbContext.ChargingPiles.Where(_ => _.StationId == stationId).ToList();
        }

        [HttpPost]
        [Route("subscribe")]
        public IHttpActionResult Subscribe(dynamic obj)
        {
            string userId = obj.userId;
            string sn = obj.sn;

            var exists = SysDbContext.ChargingPiles.Any(_ => _.SerialNumber == sn);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("编号不存在！"));
            }

            try
            {
                var theCPile = SysDbContext.ChargingPiles.Where(_ => _.SerialNumber == sn).First();
                if (theCPile.Status !=  ChargingPileStatu.Free)
                {
                    HisDbContext.SubscribeRecords.Add(new SubscribeRecord()
                    {
                        SerialNumber = sn,
                        SubscribeDate = DateTime.Now,
                        SubscribeStatus = "预约失败",
                        CustomerId = userId,
                    });
                    HisDbContext.SaveChanges();
                    return Ok(SimpleResult.Failed("无法预约！"));
                }
                else
                {
                    theCPile.Status = ChargingPileStatu.Appointmented;
                    SysDbContext.SaveChanges();

                    HisDbContext.SubscribeRecords.Add(new SubscribeRecord()
                    {
                        SerialNumber = sn,
                        SubscribeDate = DateTime.Now,
                        SubscribeStatus = "预约成功",
                        CustomerId = userId,
                    });

                    HisDbContext.SaveChanges();
                }

                return Ok(SimpleResult.Succeed("预约成功！"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("预约失败！"));
        }
    }
}
