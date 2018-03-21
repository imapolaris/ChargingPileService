using ChargingPileService.Models;
using CPS.Infrastructure.Enums;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.Manager;
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
        public IHttpActionResult Login(dynamic obj)
        {
            string telephone = obj.telephone;
            string pwd = obj.pwd;
            CustomerTypeEnum cType = obj.cType;

            try
            {
                if (cType == CustomerTypeEnum.Personal)
                {
                    var customer = SysDbContext.PersonalCustomers.Where(_ => _.Telephone == telephone && _.Password == pwd).FirstOrDefault();
                    if (customer != null)
                    {
                        return Ok(new Models.SingleResult<PersonalCustomer>(true, "登录成功！", new PersonalCustomer
                        {
                            Id = customer.Id,
                            Telephone = customer.Telephone,
                            Avatar = customer.Avatar,
                            Sex = customer.Sex,
                            NickName = customer.NickName,
                        }));
                    }
                }
                else
                {
                    var gcustomer = SysDbContext.GroupCustomers.Where(_ => _.Telephone == telephone).FirstOrDefault();
                    if (gcustomer != null)
                    {
                        return Ok(new Models.SingleResult<GroupCustomer>(true, "登录成功！", new GroupCustomer
                        {
                            Id = gcustomer.Id,
                            Telephone = gcustomer.Telephone,
                            Sex = gcustomer.Sex,
                            Avatar = gcustomer.Avatar,
                            NickName = gcustomer.NickName,
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("登录失败！"));
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(dynamic obj)
        {
            string telephone = obj.telephone;
            string vcode = obj.vcode;
            string pwd = obj.pwd;

            var exists = SysDbContext.PersonalCustomers.Any(_ => _.Telephone == telephone);
            if (exists)
            {
                return Ok(SimpleResult.Failed("手机号已注册！"));
            }

            var valid = SmsServiceConfig.Instance.ValidateVCode(telephone, vcode);
            if (valid)
            {
                var customer = new PersonalCustomer
                {
                    Telephone = telephone,
                    Password = pwd,
                    NickName = telephone,
                };
                SysDbContext.PersonalCustomers.Add(customer);
                int result = SysDbContext.SaveChanges();

                if (result > 0)
                {
                    // 添加钱包信息
                    SysDbContext.Wallets.Add(new Sys_Wallet
                    {
                        CustomerId = customer.Id,
                        Remaining = 0,
                    });
                    SysDbContext.SaveChangesAsync();

                    return Ok(SimpleResult.Succeed("注册成功，请登录！"));
                }
            }

            return Ok(SimpleResult.Failed("注册失败！"));
        }

        [HttpPost]
        [Route("reset")]
        public IHttpActionResult ResetPwd(dynamic obj)
        {
            string telephone = obj.telephone;
            string pwd = obj.pwd;
            string vcode = obj.vcode;

            var exists = SysDbContext.PersonalCustomers.Any(_ => _.Telephone == telephone);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("手机号不存在！"));
            }

            var valid = SmsServiceConfig.Instance.ValidateVCode(telephone, vcode);
            if (valid)
            {
                try
                {
                    var theCustomer = SysDbContext.PersonalCustomers.Where(_ => _.Telephone == telephone).FirstOrDefault();
                    if (theCustomer != null)
                    {
                        theCustomer.Password = pwd;
                        SysDbContext.SaveChanges();

                        return Ok(SimpleResult.Succeed("重置成功！"));
                    }
                    else
                    {
                        var theGCustomer = SysDbContext.GroupCustomers.Where(_ => _.Telephone == telephone).FirstOrDefault();
                        if (theGCustomer != null)
                        {
                            theGCustomer.Password = pwd;
                            SysDbContext.SaveChanges();

                            return Ok(SimpleResult.Succeed("重置成功！"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return Ok(SimpleResult.Failed("重置失败！"));
        }

        // 检索个人信息
        [HttpPost]
        [Route("info")]
        public IHttpActionResult GetUserProfile(dynamic obj)
        {
            string userId = obj.userId;
            try
            {
                var customer = SysDbContext.PersonalCustomers.Where(_ => _.Id == userId).FirstOrDefault();
                if (customer != null)
                {
                    return Ok(new Models.SingleResult<PersonalCustomer>(true, "操作完成！", new PersonalCustomer
                    {
                        Id = customer.Id,
                        NickName = customer.NickName,
                        Sex = customer.Sex,
                        Avatar = customer.Avatar,
                        Telephone = customer.Telephone,
                    }));
                }
                else
                {
                    var gcustomer = SysDbContext.GroupCustomers.Where(_ => _.Id == userId).FirstOrDefault();
                    if (gcustomer != null)
                    {
                        return Ok(new Models.SingleResult<GroupCustomer>(true, "操作完成！", new GroupCustomer
                        {
                            Id = customer.Id,
                            NickName = customer.NickName,
                            Sex = customer.Sex,
                            Avatar = customer.Avatar,
                            Telephone = customer.Telephone,
                        }));
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(new SimpleResult(false, "无法获取用户信息！"));
            }
        }

        // 更新个人信息
        [HttpPost]
        [Route("update")]
        public IHttpActionResult UpdateUserProfile(PersonalCustomer customer)
        {
            var exists = SysDbContext.PersonalCustomers.Any(_ => _.Id == customer.Id);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("用户不存在！"));
            }

            try
            {
                var theCustomer = SysDbContext.PersonalCustomers.Where(_ => _.Id == customer.Id).FirstOrDefault();
                if (theCustomer != null)
                {
                    theCustomer.Avatar = customer.Avatar;
                    theCustomer.NickName = customer.NickName;
                    theCustomer.Sex = customer.Sex;
                    SysDbContext.SaveChanges();

                    return Ok(SimpleResult.Succeed("修改成功！"));
                }
                else
                {
                    var theGCustomer = SysDbContext.GroupCustomers.Where(_ => _.Id == customer.Id).FirstOrDefault();
                    if (theGCustomer != null)
                    {
                        theGCustomer.Avatar = customer.Avatar;
                        theGCustomer.NickName = customer.NickName;
                        theGCustomer.Sex = customer.Sex;
                        SysDbContext.SaveChanges();

                        return Ok(SimpleResult.Succeed("修改成功！"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("修改失败！"));
        }

        [HttpPost]
        [Route("change")]
        [Obsolete]
        public IHttpActionResult ChangePwd(dynamic obj)
        {
            string userId = obj.id;
            string oldPwd = obj.oldPwd;
            string newPwd = obj.newPwd;

            var exists = SysDbContext.PersonalCustomers.Any(_ => _.Id == userId && _.Password == oldPwd);
            if (!exists)
            {
                return Ok(SimpleResult.Failed("原密码不正确！"));
            }

            try
            {
                var customer = SysDbContext.PersonalCustomers.Where(_ => _.Id == userId && _.Password == oldPwd).First();
                customer.Password = newPwd;
                SysDbContext.SaveChanges();

                return Ok(SimpleResult.Succeed("修改密码成功！"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("修改密码失败！"));
        }
    }
}
