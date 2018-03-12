using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ChargingPileWCFService
{
    using Soaring.WebMonter.DB;
    using Soaring.WebMonter.Contract.History;
    using CPS.Infrastructure.Utils;

    public class CPService : ICPService
    {
        public bool UploadBatteryTestingReport(BatteryTestringReport report)
        {
            if (report == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(report.CustomerId)
                || string.IsNullOrEmpty(report.VehicleNo))
                throw new ArgumentException();

            var dbContext = new HistoryDbContext();
            var dbTrans = dbContext.Database.BeginTransaction();
            try
            {
                var testingReport = new BatteryCheckResult()
                {
                    CheckDate = report.CheckDate,
                    CustomerId = report.CustomerId,
                    PlateNo = report.VehicleNo,
                    Results = report.Results,
                };

                var reportDetail = new BatteryCheckResultDetail()
                {
                    ADiff = report.ADiff,
                    VDiff = report.VDiff,
                    CAN = report.CAN,
                    Capacity = report.Capacity,
                    DCR = report.DCR,
                    IR = report.IR,
                    MaintainProposal = report.Proposal,
                    ReportId = testingReport.Id,
                    Temp = report.Temp,
                    TempUp = report.TempUp,
                };

                testingReport.DetailId = reportDetail.Id;
                dbContext.BatteryCheckResults.Add(testingReport);

                dbContext.BatteryCheckResultDetails.Add(reportDetail);

                int result = dbContext.SaveChanges();

                dbTrans.Commit();

                if (result > 0)
                    return true;
            }
            catch (Exception ex)
            {
                dbTrans.Rollback();
                Logger.Error("WCFService: " + ex.Message);
            }

            return false;
        }
    }
}
