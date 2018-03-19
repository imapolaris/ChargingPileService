using ChargingPileService.Models;
using Soaring.WebMonter.Contract.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    using CPS.Infrastructure.Enums;
    using CPS.Infrastructure.Models;
    using CPS.Infrastructure.Utils;
    using Soaring.WebMonter.Contract.Procedure;
    using System.Data.SqlClient;

    [RoutePrefix("api/charging")]
    public class ChargingController : MqOperatorBase
    {
        [HttpPost]
        [Route("start")]
        public async Task<IHttpActionResult> StartCharging(dynamic obj)
        {
            try
            {
                string sn = obj.sn;
                string userId = obj.userId;
                CustomerTypeEnum cType = obj.cType;

                // 检查充电桩编号是否合法
                if (!ValidSerialNumber(sn))
                {
                    return Ok(SimpleResult.Failed("充电桩编号不存在！"));
                }

                // 验证用户的有效性
                if (cType == CustomerTypeEnum.Personal)
                {
                    var result = SysDbContext.PersonalCustomers.Any(_ => _.Id == userId);
                    if (result)
                    {
                        // 个人用户检查账户余额是否充足
                        var wallet = SysDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                        if (wallet == null || wallet.Remaining <= Math.Pow(10, -6))
                        {
                            return Ok(SimpleResult.Failed("账户余额不足，请充值！"));
                        }
                    }
                    else
                    {
                        return Ok(SimpleResult.Failed("登录失效，请重新登录！"));
                    }
                }
                else
                {
                    var result = SysDbContext.GroupCustomers.Any(_ => _.Id == userId);
                    if (!result)
                    {
                        return Ok(SimpleResult.Failed("登录失效，请重新登录！"));
                    }
                }

                // 开始请求充电
                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", ActionTypeEnum.Startup);
                data.SetValue("sn", sn);
                data.SetValue("port", 0);
                data.SetValue("money", 0);
                CallAsync(data.ToJson());
                
                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var sessionResult = session.GetSessionResult();
                    var result = (ResultTypeEnum)sessionResult?.GetByteValue("result");
                    if (result == ResultTypeEnum.Succeed)
                    {
                        var record = new ChargRecord()
                        {
                            CustomerId = userId,
                            ChargingDate = DateTime.Now,
                            CPSerialNumber = sn,
                            StartDate = DateTime.Now,
                            Transactionsn = sessionResult?.GetLongValue("transSn").ToString() ?? "0",
                            IsSucceed = result == ResultTypeEnum.Succeed,
                        };

                        // 防止操作数据库期间发生错误，而此时已经开始充电，APP却得到错误的状态。
                        try
                        {
                            HisDbContext.ChargingRecords.Add(record);
                            HisDbContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        return Ok(Models.SingleResult<ChargRecord>.Succeed("已开始充电！", record));
                    }
                    else
                    {
                        Logger.Error("启动充电失败！");
                        return Ok(SimpleResult.Failed("启动充电失败！"));
                    }
                }
                else
                {
                    Logger.Error("启动充电失败：超时！");
                    return Ok(SimpleResult.Failed("启动充电失败，请求超时！"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("启动充电失败！"));
            }
        }

        [HttpPost]
        [Route("stop")]
        public async Task<IHttpActionResult> StopCharging(dynamic obj)
        {
            try
            {
                string userId = obj.userId;
                string sn = obj.sn;
                string transSn = obj.transSn;

                if (!ValidSerialNumber(sn))
                {
                    return Ok(SimpleResult.Failed("充电桩编号不存在！"));
                }

                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", ActionTypeEnum.Shutdown);
                data.SetValue("sn", sn);
                data.SetValue("transSn", transSn);
                data.SetValue("port", 0);
                CallAsync(data.ToJson());

                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var result = (ResultTypeEnum)session.GetSessionResult()?.GetByteValue("result");
                    if (result == ResultTypeEnum.Succeed)
                    {
                        // 防止操作数据库期间发生错误，而此时已经结束充电，APP却得到错误的状态。
                        try
                        {
                            var record = HisDbContext.ChargingRecords.Where(_ => _.CPSerialNumber == sn && _.Transactionsn == transSn).FirstOrDefault();
                            // 如果开始充电时没有记录到数据库，在这里补充一条记录。
                            if (record == null)
                            {
                                record = new ChargRecord()
                                {
                                    CustomerId = userId,
                                    ChargingDate = DateTime.Now,
                                    CPSerialNumber = sn,
                                    //StartDate = DateTime.Now,
                                    Cost = 0,
                                    Kwhs = 0,
                                    IsSucceed = true,
                                    EndDate = DateTime.Now,
                                };
                                HisDbContext.ChargingRecords.Add(record);
                                HisDbContext.SaveChangesAsync();
                            }
                            else
                            {
                                record.EndDate = DateTime.Now;
                                HisDbContext.SaveChangesAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }

                        return Ok(SimpleResult.Succeed("已结束充电！"));
                    }
                    else
                    {
                        Logger.Error("结束充电失败！");
                        return Ok(SimpleResult.Failed("结束充电失败！"));
                    }
                }
                else
                {
                    Logger.Error("结束充电失败：超时！");
                    return Ok(SimpleResult.Failed("结束充电失败，请求超时！"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("结束充电失败！"));
            }
        }

        [HttpGet]
        [Route("status")]
        public IHttpActionResult RequestChargingStatus(string sn, string transSn)
        {
            if (!ValidSerialNumber(sn))
            {
                return Ok(SimpleResult.Failed("充电桩编号不存在！"));
            }

            try
            {
                var db = _redis.GetDatabase();
                var rtData = db.HashGet(Constants.ChargingRealtimeDataContainerKey, sn);
                if (!rtData.IsNullOrEmpty)
                {
                    var data = new UniversalData();
                    data.FromJson(rtData);
                    var tSn = data.GetStringValue("transSn");
                    if (tSn == transSn)
                    {
                        return Ok(Models.SingleResult<string>.Succeed("查询成功！", rtData));
                    }
                    else // 充电是否已结束？
                    {
                        return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
                    }
                }
                else
                {
                    Logger.Info("查询充电桩状态失败：充电桩没有反馈！");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
        }

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult GetChargingSummary(string userId)
        {
            var sqlParms = new SqlParameter[1];
            sqlParms[0] = new SqlParameter("@userId", userId);
            var result = HisDbContext.Database.SqlQuery(typeof(ChargingSummary), "exec sp_customerChargingDataStatistics @userId", sqlParms).Cast<ChargingSummary>().FirstOrDefault();

            if (result == null)
                return Ok(SimpleResult.Failed("查询失败！"));
            else
                return Ok(Models.SingleResult<ChargingSummary>.Succeed("查询成功！", result));
        }

        [HttpGet]
        [Route("records")]
        public IEnumerable<ChargRecord> GetChargingRecords(string userId)
        {
            return HisDbContext.ChargingRecords.Where(_=>_.CustomerId == userId).OrderByDescending(_=>_.StartDate);
        }

        [NonAction]
        private bool ValidSerialNumber(string serialNumber)
        {
            return SysDbContext.ChargingPiles.Any(_ => _.SerialNumber == serialNumber);
        }
    }
}
