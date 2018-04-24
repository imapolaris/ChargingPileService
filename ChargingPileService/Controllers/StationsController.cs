using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChargingPileService.Models;
using Soaring.WebMonter.Contract.Manager;
using CPS.Infrastructure.Models;
using Soaring.WebMonter.Contract.History;
using CPS.Infrastructure.Enums;
using Newtonsoft.Json.Linq;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/stations")]
    public class StationsController : MqOperatorBase
    {
        public IEnumerable<Sys_Station> GetAllStations()
        {
            // TODO：电站数据改成从缓存中获取。
            return SysDbContext.Stations;
        }

        [HttpGet]
        public IHttpActionResult GetStationInfoById(string stationId)
        {
            Sys_Station station = null;

            try
            {
                var exists = SysDbContext.Stations.Any(_ => _.Id == stationId);
                if (exists)
                {
                    station = SysDbContext.Stations.Where(_ => _.Id == stationId).First();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (station != null)
                return Ok(new Models.SingleResult<Sys_Station>(true, "查找到电站信息！", station));
            else
                return Ok(SimpleResult.Failed("无法查询电站信息！"));
        }

        [HttpGet]
        [Route("details")]
        public IHttpActionResult GetStationDetailInfoById(string userId, string stationId)
        {
            Sys_Station station = null;

            try
            {
                var exists = SysDbContext.Stations.Any(_ => _.Id == stationId);
                if (exists)
                {
                    station = SysDbContext.Stations.Include("Detail").Where(_ => _.Id == stationId).First();
                    var c_s = HisDbContext.CustomerStations.Where(_ => _.CustomerId == userId && _.StationId == stationId).FirstOrDefault();
                    if (c_s == null)
                        station.C_S = new CustomerStation();
                    else
                        station.C_S = c_s;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (station != null)
                return Ok(new Models.SingleResult<Sys_Station>(true, "查找到电站信息！", station));
            else
                return Ok(SimpleResult.Failed("无法查询电站信息！"));
        }

        [Route("search/{filter}")]
        public IHttpActionResult GetStationNames(string filter)
        {
            var stations = SysDbContext.Stations.Where(_ => _.Name.Contains(filter) || _.Address.Contains(filter));
            return Ok(stations);
        }

        [HttpGet]
        [Route("nearby")]
        public IHttpActionResult GetNearbyStations(double lon, double lat)
        {
            if (!MapHelper.IsValidCoord(lon, lat))
            {
                return NotFound();
            }
            var lngLat = new LngLat()
            {
                Longitude = lon,
                Latitude = lat,
            };

            var stations = SysDbContext.Stations.AsEnumerable()
                .Where(_ => lngLat.IsNearby(new LngLat { Longitude = _.Lon, Latitude = _.Lat })).ToList();

            stations.ForEach(_ => _.Distance = lngLat.Between2Coords(new LngLat { Longitude = _.Lon, Latitude = _.Lat }));
            return Ok(stations.OrderBy(_ => _.Distance));
        }

        #region 【收藏电站】

        [Route("collect/{userId}")]
        public IEnumerable<Sys_Station> GetCollectStations(string userId)
        {
            return HisDbContext.CustomerStations.Where(_ => _.CustomerId == userId && _.IsCollect)
                .ToList()
                .Join(SysDbContext.Stations, a => a.StationId, b => b.Id, (a, b) => b);
        }

        [HttpPost]
        [Route("collect/state")]
        public IHttpActionResult StationCollectState(dynamic obj)
        {
            string userId = obj.userId;
            string stationId = obj.stationId;
            var entities = HisDbContext.CustomerStations.Where(_ => _.CustomerId == userId && _.StationId == stationId);
            var message = "收藏成功";
            if (entities != null && entities.Count() > 0)
            {
                foreach (var entity in entities)
                {
                    entity.IsCollect = !entity.IsCollect;
                    if (!entity.IsCollect)
                        message = "取消收藏";
                }
            }
            else
            {
                HisDbContext.CustomerStations.Add(new CustomerStation()
                {
                    CustomerId = userId,
                    StationId = stationId,
                    IsCollect = true,
                    IsValid = true,
                    ChargingTimes = 0,
                });
            }

            HisDbContext.SaveChanges();
            return Ok(SimpleResult.Succeed(message));
        }

        [HttpPost]
        [Route("collect/clear")]
        public IHttpActionResult ClearCollectStations(dynamic obj)
        {
            string userId = obj.userId;

            var entities = HisDbContext.CustomerStations.Where(_ => _.CustomerId == userId && _.IsCollect);
            if (entities != null && entities.Count() > 0)
            {
                foreach (var entity in entities)
                {
                    entity.IsCollect = false;
                }

                HisDbContext.SaveChanges();
            }
            return Ok(SimpleResult.Succeed("操作成功！"));
        }

        #endregion 【收藏电站】

        #region 【下发配置】

        /// <summary>
        /// 下发电价/服务费
        /// </summary>
        [HttpPost]
        [Route("setting/price")]
        public IHttpActionResult SetPrices(dynamic obj)
        {
            string stationId = obj.stationId;
            if (string.IsNullOrEmpty(stationId))
            {
                Logger.Info("下发费率，站点Id不能为空。");
                return Ok(SimpleResult.Failed("设置失败！"));
            }

            int priceType = obj.priceType; // 0-电价，1-服务费
            int sr = obj.sr;
            int pr = obj.pr;
            int fr = obj.fr;
            int vr = obj.vr;

            UniversalData data = new UniversalData();
            data.SetValue("id", Guid.NewGuid().ToString());
            data.SetValue("oper", priceType == 0 ? ActionTypeEnum.SetElecPrice : ActionTypeEnum.SetServicePrice);
            data.SetValue("stationId", stationId);
            data.SetValue("priceType", priceType);
            data.SetValue("sr", sr);
            data.SetValue("pr", pr);
            data.SetValue("fr", fr);
            data.SetValue("vr", vr);
            CallAsync(data.ToJson());

            return Ok(SimpleResult.Succeed("设置成功！"));
        }


        /// <summary>
        /// 下发尖峰平谷时间段
        /// </summary>
        [HttpPost]
        [Route("setting/period")]
        public IHttpActionResult SetPeriod(dynamic obj)
        {
            string stationId = obj.stationId;
            if (string.IsNullOrEmpty(stationId))
            {
                Logger.Info("下发尖峰平谷时间段，站点Id不能为空。");
                return Ok(SimpleResult.Failed("设置失败！"));
            }

            byte periodType = obj.periodType; // 0--电价，1--服务费

            byte sr = obj.sr;
            JArray srs = obj.srs;
            byte pr = obj.pr;
            JArray prs = obj.prs;
            byte fr = obj.fr;
            JArray frs = obj.frs;
            byte vr = obj.vr;
            JArray vrs = obj.vrs;

            UniversalData data = new UniversalData();
            data.SetValue("id", Guid.NewGuid().ToString());
            data.SetValue("oper", ActionTypeEnum.SetPeriod);
            data.SetValue("stationId", stationId);
            data.SetValue("periodType", periodType);
            data.SetValue("sr", sr);
            data.SetValue("srs", srs);
            data.SetValue("pr", pr);
            data.SetValue("prs", prs);
            data.SetValue("fr", fr);
            data.SetValue("frs", frs);
            data.SetValue("vr", vr);
            data.SetValue("vrs", vrs);
            
            CallAsync(data.ToJson());

            return Ok(SimpleResult.Succeed("设置成功！"));
        }

        #endregion
    }
}
