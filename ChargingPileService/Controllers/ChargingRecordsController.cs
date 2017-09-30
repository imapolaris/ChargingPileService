using CPS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class ChargingRecordsController : OperatorBase
    {
        public IEnumerable<ChargingRecord> GetAllRecords(string id)
        {
            return EntityContext.CPS_ChargingRecord.Where(_=>_.UserId == id);
        }
    }
}
