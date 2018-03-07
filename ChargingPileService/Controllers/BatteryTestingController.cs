using ChargingPileService.Models;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/batterytesting")]
    public class BatteryTestingController : OperatorBase
    {
        [HttpGet]
        public IEnumerable<BatteryCheckResult> GetBatteryTestingReports(string userId)
        {
            return HisDbContext.BatteryCheckResults.Where(_ => _.CustomerId == userId).ToList();
        }

        [HttpGet]
        public IHttpActionResult GetBatteryTestingReportDetail(string reportId)
        {
            var result = HisDbContext.BatteryCheckResultDetails.Where(_ => _.ReportId == reportId).FirstOrDefault();
            return Ok(new Models.SingleResult<BatteryCheckResultDetail>(true, "查找到报告详情！", result));
        }

        [HttpPost]
        [Route("submit")]
        public IHttpActionResult SubmitBatteryTestingReport(BatteryCheckResult report)
        {
            return Ok();
        }

        [HttpPost]
        [Route("start")]
        public IHttpActionResult StartBatteryTesting()
        {
            return Ok();
        }

        [HttpPost]
        [Route("stop")]
        public IHttpActionResult StopBatteryTesting()
        {
            return Ok();
        }

        [HttpPost]
        [Route("monitor")]
        public IHttpActionResult QueryBatteryTestingProcess(string processId)
        {
            return Ok();
        }

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult GetBatteryTestingSummary()
        {
            return null;
        }

        [HttpGet]
        [Route("records")]
        public IEnumerable<BatteryCheckRecord> GetBatteryTestingRecords()
        {
            return null;
        }
    }
}
