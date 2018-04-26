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

            // for test.
            // Logger.Info($"stationId:{stationId}, 类型：{priceType} sr:{sr}, pr:{pr}, fr:{fr}, vr:{vr}");

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

            JArray srs = obj.srs;
            byte sr = (byte)(srs.Count);
            var srsT = new JArray();
            foreach (var item in srs)
            {
                var t = item.Value<byte>();
                srsT.Add(t);
                srsT.Add(t + 1);
            }
            JArray prs = obj.prs;
            byte pr = (byte)(prs.Count);
            var prsT = new JArray();
            foreach (var item in prs)
            {
                var t = item.Value<byte>();
                prsT.Add(t);
                prsT.Add(t + 1);
            }
            JArray frs = obj.frs;
            byte fr = (byte)(frs.Count);
            var frsT = new JArray();
            foreach (var item in frs)
            {
                var t = item.Value<byte>();
                frsT.Add(t);
                frsT.Add(t + 1);
            }
            JArray vrs = obj.vrs;
            byte vr = (byte)(vrs.Count);
            var vrsT = new JArray();
            foreach (var item in vrs)
            {
                var t = item.Value<byte>();
                vrsT.Add(t);
                vrsT.Add(t + 1);
            }

            // for test.
            /*
            Logger.Info($"stationId:{stationId}, 类型：{periodType}, sr:{sr}, pr:{pr}, fr:{fr}, vr:{vr}");
            var temp = "";
            foreach (var item in srsT)
            {
                temp += item.Value<byte>().ToString() + ",";
            }
            Logger.Info($"srs:{temp}");
            temp = "";
            foreach (var item in prsT)
            {
                temp += item.Value<byte>().ToString() + ",";
            }
            Logger.Info($"prs:{temp}");
            temp = "";
            foreach (var item in frsT)
            {
                temp += item.Value<byte>().ToString() + ",";
            }
            Logger.Info($"frs:{temp}");
            temp = "";
            foreach (var item in vrsT)
            {
                temp += item.Value<byte>().ToString() + ",";
            }
            Logger.Info($"srs:{temp}");
            */

            UniversalData data = new UniversalData();
            data.SetValue("id", Guid.NewGuid().ToString());
            data.SetValue("oper", ActionTypeEnum.SetPeriod);
            data.SetValue("stationId", stationId);
            data.SetValue("periodType", periodType);
            data.SetValue("sr", sr);
            data.SetValue("srs", srsT);
            data.SetValue("pr", pr);
            data.SetValue("prs", prsT);
            data.SetValue("fr", fr);
            data.SetValue("frs", frsT);
            data.SetValue("vr", vr);
            data.SetValue("vrs", vrsT);

            CallAsync(data.ToJson());

            return Ok(SimpleResult.Succeed("设置成功！"));
        }

        #endregion
    }
}
