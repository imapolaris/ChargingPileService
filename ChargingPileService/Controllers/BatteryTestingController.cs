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
            var results = HisDbContext.BatteryCheckResults.Where(_ => _.CustomerId == userId).ToList();
            var ids = results.Select(_ => _.VehicleId).ToList();
            var vs = SysDbContext.VehicleInfoes.Where(p => ids.Contains(p.Id)).ToList();

            foreach (var x in results)
            {
                x.Vehicle = vs.FirstOrDefault(p => x.VehicleId == p.Id);
            }

            return results;
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
    }
}
