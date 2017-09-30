using ChargingPileService.Models;
using CPS.DB;
using CPS.Entities;
using CPS.Infrastructure.Utils;
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
            try
            {
                var exists = EntityContext.CPS_User.Any(_ => _.PhoneNumber == user.PhoneNumber && _.Password == user.Password);
                if (exists)
                {
                    var theUser = EntityContext.CPS_User.Where(_ => _.PhoneNumber == user.PhoneNumber && _.Password == user.Password).First();
                    return Ok(new Models.SingleResult<User>(true, "登录成功！", new CPS.Entities.User
                    {
                        Id = theUser.Id,
                        PhoneNumber = theUser.PhoneNumber,
                        Avatar = theUser.Avatar,
                        Gender = theUser.Gender,
                        NickName = theUser.NickName,
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }

            return Ok(SimpleResult.Failed("手机号或密码错误！"));
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(User user)
        {
            var exists = EntityContext.CPS_User.Any(_ => _.PhoneNumber == user.PhoneNumber);
            if (exists)
            {
                return Ok(SimpleResult.Failed("手机号已注册！"));
            }

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

                return Ok(SimpleResult.Succeed("注册成功，请登录！"));
            }

            return Ok(SimpleResult.Failed("注册失败！"));
        }

        [HttpPost]
        [Route("reset")]
        public IHttpActionResult ResetPwd(User user)
        {
            var exists = EntityContext.CPS_User.Any(_ => _.PhoneNumber == user.PhoneNumber);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("手机号不存在！"));
            }

            var valid = SmsServiceConfig.Instance.ValidateVCode(user.PhoneNumber, user.VCode);
            if (valid)
            {
                try
                {
                    var theUser = EntityContext.CPS_User.Where(_ => _.PhoneNumber == user.PhoneNumber).First();
                    theUser.Password = user.Password;
                    EntityContext.SaveChanges();

                    return Ok(SimpleResult.Succeed("重置密码成功！"));
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }

            return Ok(SimpleResult.Failed("重置密码失败！"));
        }

        // 检索个人信息
        [HttpPost]
        [Route("info")]
        public IHttpActionResult GetUserProfile(User user)
        {
            try
            {
                var theUser = EntityContext.CPS_User.Where(_ => _.Id == user.Id).First();

                return Ok(new Models.SingleResult<User>(true, "", new User
                {
                    Id = theUser.Id,
                    NickName = theUser.NickName,
                    Gender = theUser.Gender,
                    Avatar = theUser.Avatar,
                    PhoneNumber = theUser.PhoneNumber,
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                return Ok(new SimpleResult(false, "无法获取用户信息！"));
            }
        }

        // 更新个人信息
        [HttpPost]
        [Route("update")]
        public IHttpActionResult UpdateUserProfile(User user)
        {
            var exists = EntityContext.CPS_User.Any(_ => _.Id == user.Id);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("用户不存在！"));
            }

            try
            {
                var theUser = EntityContext.CPS_User.Where(_ => _.Id == user.Id).First();
                theUser.Avatar = user.Avatar;
                theUser.NickName = user.NickName;
                theUser.Gender = user.Gender;
                EntityContext.SaveChanges();

                return Ok(SimpleResult.Succeed("修改个人资料成功！"));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                return Ok(SimpleResult.Failed("修改个人资料失败！"));
            }
        }

        [HttpPost]
        [Route("change")]
        public IHttpActionResult ChangePwd(User user)
        {
            var exists = EntityContext.CPS_User.Any(_ => _.Id == user.Id && _.Password == user.Password);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("原密码不正确！"));
            }

            try
            {
                var theUser = EntityContext.CPS_User.Where(_ => _.Id == user.Id && _.Password == user.Password).First();
                theUser.Password = user.NewPassword;
                EntityContext.SaveChanges();

                return Ok(SimpleResult.Succeed("修改密码成功！"));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }

            return Ok(SimpleResult.Failed("修改密码失败！"));
        }
    }
}
