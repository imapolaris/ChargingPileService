using ChargingPileService.Models;
using CPS.DB;
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
    public class UsersController : OperatorBase
    {
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(User user)
        {
            var exists = EntityContext.CPS_User.Any(_ => _.PhoneNumber == user.PhoneNumber && _.Password == user.Password);
            if (exists)
            {
                var returnVal = new SimpleResult(true, "登录成功！");
                return Ok(returnVal);
            }
            else
            {
                var returnVal = new SimpleResult(false, "手机号或密码错误！");
                return Ok(returnVal);
            }
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(User user)
        {
            var valid = SmsServiceConfig.Instance.ValidateVCode(user.PhoneNumber, user.VCode);
            if (valid)
            {
                EntityContext.CPS_User.Add(new User
                {
                    PhoneNumber = user.PhoneNumber,
                    Password = user.Password,
                    NickName = user.PhoneNumber,
                });
                EntityContext.SaveChanges();

                return Ok(new SimpleResult(true, "注册成功，请登录！"));
            }

            return Ok(new SimpleResult(false, "注册失败！"));
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
        [Route("update")]
        public IHttpActionResult UpdateUserProfile(User user)
        {
            return Ok(true);
        }
    }
}
