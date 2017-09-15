using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChargingPileService.Models;

namespace ChargingPileService.Controllers
{
    public class SubscribeRecordsController : ApiController
    {
        SubscribeRecord[] records = new SubscribeRecord[]
        {
            new SubscribeRecord
            {
                Id="1",
                SerialNumber="100023",
                SubscribeDate="2017-08-04 09:40:33",
                SubscribeStatus="预约成功",
            },
            new SubscribeRecord
            {
                Id="2",
                SerialNumber="100024",
                SubscribeDate="2017-08-04 09:40:33",
                SubscribeStatus="预约成功",
            },
            new SubscribeRecord
            {
                Id="3",
                SerialNumber="100025",
                SubscribeDate="2017-08-04 09:40:33",
                SubscribeStatus="预约失败",
            },
            new SubscribeRecord
            {
                Id="4",
                SerialNumber="100026",
                SubscribeDate="2017-08-04 09:40:33",
                SubscribeStatus="预约取消",
            },
            new SubscribeRecord
            {
                Id="5",
                SerialNumber="100027",
                SubscribeDate="2017-08-04 09:40:33",
                SubscribeStatus="预约成功",
            },
        };

        public IEnumerable<SubscribeRecord> GetAllRecords()
        {
            return records;
        }
    }
}
