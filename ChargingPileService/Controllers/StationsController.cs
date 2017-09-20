using ChargingPileService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/{stations}")]
    public class StationsController : ApiController
    {
        Station[] stations = new Station[]
        {
            new Station
            {
                Id="1",
                Name="加速器一区充电站",
                Longitude=116.2499720000,
                Latitude=40.0885740000,
                Numbers="1/2",
                Address="北京市海淀区永丰产业基地加速器一区充电站",
            },
            new Station
            {
                Id="2",
                Name="滕州站充电站",
                Longitude=117.263977,
                Latitude=35.098302,
                Numbers="3/4",
                Address="山东省枣庄市滕州站",
            },
            new Station
            {
                Id="3",
                Name="徐汇区充电站",
                Longitude=121.44317,
                Latitude=31.19541,
                Numbers="0/5",
                Address="上海徐汇区徐家汇",
            },
        };

        public IEnumerable<Station> Get()
        {
            return stations;
        }

        public IHttpActionResult Get(string id)
        {
            var station = stations.FirstOrDefault(_ => _.Id == id);
            if (station == null)
                return NotFound();
            return Ok(station);
        }

        [Route("names/{name}")]
        public IEnumerable<string> Get(string name)
        {
            return stations.Where(_ => _.Name.Contains(name)).Select(_=>_.Name);
        }
    }
}
