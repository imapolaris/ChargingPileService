using CPS.Entities;
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
    }
}
