using CPS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class PayRecordsController : OperatorBase
    {
        public IEnumerable<PayRecord> GetRecordsByUserId(string id)
        {
            return EntityContext.CPS_PayRecord.Where(_=>_.UserId == id);
        }
    }
}
