using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class WalletController : OperatorBase
    {
        public IHttpActionResult Get(string userId)
        {
            return Ok();
        }
    }
}
