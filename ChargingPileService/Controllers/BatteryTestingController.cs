using ChargingPileService.Models;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.History;
using Soaring.WebMonter.Contract.Procedure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public IHttpActionResult GetBatteryTestingSummary(string userId)
        {
            SqlParameter[] sqlParms = new SqlParameter[1];
            sqlParms[0] = new SqlParameter("@userId", userId);
            var result = HisDbContext.Database.SqlQuery<BatteryTestingSummary>("exec sp_customerBatteryTestingDataStatistics @userId", sqlParms).Cast<BatteryTestingSummary>().FirstOrDefault();

            if (result == null)
                return Ok(SimpleResult.Failed("查询失败！"));
            else
                return Ok(Models.SingleResult<BatteryTestingSummary>.Succeed("查询成功！", result));
        }

        [HttpGet]
        [Route("records")]
        public IEnumerable<BatteryCheckRecord> GetBatteryTestingRecords(string userId)
        {
            return HisDbContext.BatteryCheckRecords.Where(_ => _.CustomerId == userId).OrderByDescending(_ => _.StartDate);
        }
    }
}
