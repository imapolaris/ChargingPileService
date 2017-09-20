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

        public IHttpActionResult Register(string username, string phoneNumber, string password)
        {
            return Ok(true);
        }

        public IHttpActionResult ResetPwd(string phoneNumber, string verifyCode, string newPwd)
        {
            return Ok(true);
        }

        // 保存头像
        public IHttpActionResult SaveAvatar(byte[] avatar)
        {
            return Ok(true);
        }

        // 更新个人信息
        public IHttpActionResult UpdateUserProfile()
        {
            return Ok(true);
        }
    }
}
