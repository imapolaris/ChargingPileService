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
    [RoutePrefix("api/system")]
    public class SystemController : OperatorBase
    {
        [HttpPost]
        [Route("feedback")]
        public IHttpActionResult SubmitFeedback(Feedback feedback)
        {
            try
            {
                HisDbContext.Feedbacks.Add(feedback);
                HisDbContext.SaveChanges();
                return Ok(SimpleResult.Succeed("提交成功！"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("提交失败！"));
            }
        }
    }
}
