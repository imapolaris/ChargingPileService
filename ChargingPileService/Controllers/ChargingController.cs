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
    using Common;
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

                var telephone = "";
                // 验证用户的有效性
                if (cType == CustomerTypeEnum.Personal)
                {
                    var result = SysDbContext.PersonalCustomers.Where(_ => _.Id == userId).FirstOrDefault();
                    if (result != null)
                    {
                        telephone = result.Telephone;
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
                    var result = SysDbContext.GroupCustomers.Where(_ => _.Id == userId).FirstOrDefault();
                    if (result != null)
                    {
                        telephone = result.Telephone;
                        return Ok(SimpleResult.Failed("登录失效，请重新登录！"));
                    }
                }

                // 开始请求充电
                Session session = SessionService.StartOneSession(100*1000); // 100秒延时
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", ActionTypeEnum.Startup);
                var transSn = ToolUtil.CreateTransactionSerialNumber();
                data.SetValue("transSn", transSn);
                data.SetValue("sn", sn);
                data.SetValue("port", 0);
                data.SetValue("money", 0);
                data.SetValue("userId", userId);
                data.SetValue("userName", telephone);
                CallAsync(data.ToJson());
                
                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var sessionResult = session.GetSessionResult();
                    var actionstatus = (ActionResultTypeEnum)sessionResult?.GetByteValue("actionstatus");
                    if (actionstatus == ActionResultTypeEnum.Succeed)
                    {
                        var result = (ResultTypeEnum)sessionResult?.GetByteValue("result");
                        if (result == ResultTypeEnum.Succeed)
                        {
                            var record = new ChargRecord()
                            {
                                CompanyCode = "CP_001",
                                StationCode = "SY_004",
                                CustomerId = userId,
                                ChargingDate = DateTime.Now,
                                CPSerialNumber = sn,
                                StartDate = DateTime.Now,
                                Transactionsn = transSn,
                                //IsSucceed = result == ResultTypeEnum.Succeed, // 充电完成后，设置为成功状态。
                            };

                            // 防止操作数据库期间发生错误，而此时已经开始充电，APP却得到错误的状态。
                            try
                            {
                                HisDbContext.ChargingRecords.Add(record);
                                HisDbContext.SaveChanges();
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
                        return Ok(SimpleResult.Failed("启动充电失败，请求失败！"));
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
                long transSn = obj.transSn;

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
                data.SetValue("userId", userId);
                data.SetValue("userName", "00000000000"); // redundant
                CallAsync(data.ToJson());

                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var sessionResult = session.GetSessionResult();
                    var actionstatus = (ActionResultTypeEnum)sessionResult?.GetByteValue("actionstatus");
                    if (actionstatus == ActionResultTypeEnum.Succeed)
                    {
                        var result = (ResultTypeEnum)sessionResult?.GetByteValue("result");
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
                                        CompanyCode = "CP_001",
                                        StationCode = "SY_004",
                                        CustomerId = userId,
                                        ChargingDate = DateTime.Now,
                                        CPSerialNumber = sn,
                                        StartDate = DateTime.Now,
                                        EndDate = DateTime.Now,
                                        IsSucceed = true,
                                    };
                                    HisDbContext.ChargingRecords.Add(record);
                                    HisDbContext.SaveChanges();
                                }
                                else
                                {
                                    record.EndDate = DateTime.Now;
                                    record.IsSucceed = true;
                                    HisDbContext.SaveChanges();
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
                        return Ok(SimpleResult.Failed("结束充电失败，请求失败！"));
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
                        data.SetValue("cpState", "1"); // 1-还在充电
                        return Ok(Models.SingleResult<string>.Succeed("查询成功！", data.ToJson()));
                    }
                    else // 充电是否已结束？ 
                    {
                        var record = HisDbContext.ChargingRecords.Where(_ => _.CPSerialNumber == sn && _.Transactionsn == Convert.ToInt64(transSn)).FirstOrDefault();
                        if (record == null || !record.IsSucceed)
                        {
                            return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
                        }
                        else
                        {
                            data.SetValue("cpState", "2"); // 2-充电已结束
                            return Ok(Models.SingleResult<string>.Succeed("充电完成！", data.ToJson()));
                        }
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

        [HttpGet]
        [Route("billing")]
        public async Task<IHttpActionResult> GetCurrentChargingBilling(string sn, long transSn)
        {
            // 检查充电桩编号是否合法
            if (!ValidSerialNumber(sn))
            {
                return Ok(SimpleResult.Failed("充电桩编号不存在！"));
            }

            // 首先，查询数据库中是否已存在完成账单
            var records = HisDbContext.ChargingRecords.Where(_ => _.CPSerialNumber == sn && _.Transactionsn == transSn && _.IsSucceed);
            if (records != null && records.Count() > 0)
            {
                var record = records.FirstOrDefault();
                return Ok(Models.SingleResult<string>.Succeed("请求成功！", JsonHelper.Serialize(record)));
            }
            else
            {
                // 向充电桩请求充电账单
                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", ActionTypeEnum.QueryChargingBilling);
                data.SetValue("transSn", transSn);
                data.SetValue("sn", sn);
                CallAsync(data.ToJson());

                try
                {
                    var status = await session.WaitSessionCompleted();
                    if (status)
                    {
                        var sessionResult = session.GetSessionResult();
                        var actionstatus = (ActionResultTypeEnum)sessionResult?.GetByteValue("actionstatus");
                        // 请求成功
                        if (actionstatus == ActionResultTypeEnum.Succeed)
                        {
                            return Ok(Models.SingleResult<string>.Succeed("请求成功！", sessionResult.ToJson()));
                        }
                    }
                    else
                    {
                        Logger.Error("请求失败，超时！");
                        return Ok(SimpleResult.Failed("请求失败，超时！"));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            Logger.Info($"查询账单失败：SN：{sn}，TSN：{transSn}");
            return Ok(SimpleResult.Failed("请求失败！"));
        }

        [NonAction]
        private bool ValidSerialNumber(string serialNumber)
        {
            return SysDbContext.ChargingPiles.Any(_ => _.SerialNumber == serialNumber);
        }
    }
}
