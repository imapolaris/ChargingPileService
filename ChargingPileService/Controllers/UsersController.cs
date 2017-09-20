using CPS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(User user)
        {
            if (user.UserName == "s" && user.Password == "s")
            {
                return Ok(new Models.SingleResult<bool>(true));
            }
            else
            {
                return Ok(new Models.SingleResult<bool>(false));
            }
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(User user)
        {

            return Ok(true);
        }

        [HttpPost]
        [Route("reset")]
        public IHttpActionResult ResetPwd(User user)
        {
            return Ok(true);
        }

        // 保存头像
        [HttpPost]
        [Route("avatar")]
        public IHttpActionResult SaveAvatar(byte[] avatar)
        {
            return Ok(true);
        }

        // 更新个人信息
        [HttpPost]
        [Route("userprofile")]
        public IHttpActionResult UpdateUserProfile()
        {
            return Ok(true);
        }
    }
}
