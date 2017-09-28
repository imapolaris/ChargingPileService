using CPS.Entities;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChargingPileService.Models;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/{stations}")]
    public class StationsController : OperatorBase
    {
        public IEnumerable<Station> GetAllStations()
        {
            return EntityContext.CPS_Station;
        }

        public IHttpActionResult GetStationById(string id)
        {
            try
            {
                var exists = EntityContext.CPS_Station.Any(_ => _.Id == id);
                if (exists)
                {
                    var station = EntityContext.CPS_Station.Where(_ => _.Id == id).First();
                    if (station == null)
                        return NotFound();
                    return Ok(new Models.SingleResult<Station>(true, "", station));
                }
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(ex);
            }

            return Ok(SimpleResult.Failed("无法查询电站信息！"));
        }

        [Route("search/{filter}")]
        public IHttpActionResult GetStationNames(string filter)
        {
            var stations = EntityContext.CPS_Station.Where(_ => _.Name.Contains(filter) || _.Address.Contains(filter));

            return Ok(stations);
        }

        [HttpPost]
        [Route("nearby")]
        public IHttpActionResult GetNearbyStations(LngLat lngLat)
        {
            if (lngLat == null
                || !MapHelper.IsValidCoord(lngLat.Longitude, lngLat.Latitude))
            {
                return NotFound();
            }

            var stations = EntityContext.CPS_Station.AsEnumerable()
                .Where(_ => lngLat.IsNearby(new LngLat { Longitude = _.Longitude, Latitude = _.Latitude })).ToList();

            stations.ForEach(_ => _.Distance = lngLat.Between2Coords(new LngLat { Longitude = _.Longitude, Latitude = _.Latitude }));

            return Ok(stations.OrderBy(_=>_.Distance));
        }
    }
}
