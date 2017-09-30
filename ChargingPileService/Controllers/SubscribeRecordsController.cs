using CPS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class SubscribeRecordsController : OperatorBase
    {
        public IEnumerable<SubscribeRecord> GetAllRecords(string id)
        {
            return EntityContext.CPS_SubscribeRecord.Where(_=>_.UserId == id);
        }
    }
}
