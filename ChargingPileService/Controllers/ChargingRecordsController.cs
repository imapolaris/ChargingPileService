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
        ChargingRecord[] records = new ChargingRecord[]
        {
            new ChargingRecord
            {
                Id="1",
                Kwhs=50,
                ChargingDate="2017-08-04 09:40:33",
                Cost=20.0,
            },
            new ChargingRecord
            {
                Id="2",
                Kwhs=50,
                ChargingDate="2017-08-04 09:40:33",
                Cost=20.0,
            },
            new ChargingRecord
            {
                Id="3",
                Kwhs=50,
                ChargingDate="2017-08-04 09:40:33",
                Cost=20.0,
            },
            new ChargingRecord
            {
                Id="4",
                Kwhs=50,
                ChargingDate="2017-08-04 09:40:33",
                Cost=20.0,
            },
            new ChargingRecord
            {
                Id="5",
                Kwhs=50,
                ChargingDate="2017-08-04 09:40:33",
                Cost=20.0,
            },
        };

        public IEnumerable<ChargingRecord> GetAllRecords(bool refreshing)
        {
            if (refreshing)
            {
                return records.Concat(records.Take(1));
            }
            return records;
        }
    }
}
