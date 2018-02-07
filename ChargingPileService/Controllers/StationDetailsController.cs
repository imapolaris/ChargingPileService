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
    [RoutePrefix("api/{stationdetails}")]
    public class StationDetailsController : OperatorBase
    {
        [HttpGet]
        public IHttpActionResult GetDetailByStationId(string id)
        {
            var exists = EntityContext.CPS_StationDetail.Any(_ => _.StationId == id);
            if (exists)
            {
                try
                {
                    var theDetail = EntityContext.CPS_StationDetail.Where(_ => _.StationId == id).First();
                    var returnVal = new Models.SingleResult<StationDetail>(true, "", theDetail);
                    return Ok(returnVal);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return NotFound();
        }
    }
}
