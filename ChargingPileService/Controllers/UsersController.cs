using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class UsersController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Login(string username, string password)
        {
            if (username == "soaring" && password == "soaring")
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
    }
}
