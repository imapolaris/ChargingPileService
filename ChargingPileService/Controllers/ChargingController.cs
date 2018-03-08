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

                if (!ValidSerialNumber(sn))
                {
                    //return Ok(SimpleResult.Failed("充电桩编号不存在！"));
                }

                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", MQMessageType.StartCharging);
                data.SetValue("sn", sn);
                data.SetValue("transSn", 1);
                data.SetValue("port", 0);
                data.SetValue("money", 0);
                CallAsync(data.ToJson());
                
                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var result = session.GetSessionResult()?.GetBooleanValue("result") ?? false;
                    var record = new ChargRecord()
                    {
                        CustomerId = userId,
                        ChargingDate = DateTime.Now,
                        CPSerialNumber = sn,
                        StartDate = DateTime.Now,
                        Cost = 0,
                        Kwhs = 0,
                        IsSucceed = result,
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

                    if (result)
                    {
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
                string recordId = "123";//obj.recordId;

                if (!ValidSerialNumber(sn))
                {
                    //return Ok(SimpleResult.Failed("充电桩编号不存在！"));
                }

                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", MQMessageType.StopCharging);
                data.SetValue("sn", sn);
                data.SetValue("transSn", 1);
                data.SetValue("port", 0);
                CallAsync(data.ToJson());

                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var result = session.GetSessionResult()?.GetBooleanValue("result") ?? false;
                    if (result)
                    {
                        // 防止操作数据库期间发生错误，而此时已经结束充电，APP却得到错误的状态。
                        try
                        {
                            var record = HisDbContext.ChargingRecords.Where(_ => _.Id == recordId).FirstOrDefault();
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
        public async Task<IHttpActionResult> RequestChargingStatus(string sn)
        {
            if (!ValidSerialNumber(sn))
            {
                //return Ok(SimpleResult.Failed("充电桩编号不存在！"));
            }

            try
            {
                Session session = SessionService.StartOneSession();
                UniversalData data = new UniversalData();
                data.SetValue("id", session.Id);
                data.SetValue("oper", MQMessageType.GetChargingPileState);
                data.SetValue("sn", sn);

                var status = await session.WaitSessionCompleted();
                if (status)
                {
                    var result = session.GetSessionResult()?.GetBooleanValue("result") ?? false;
                    if (result)
                    {
                        return Ok(SimpleResult.Succeed("查询成功！"));
                    }
                    else
                    {
                        Logger.Info("查询充电桩状态失败：充电桩没有反馈！");
                        return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
                    }
                }
                else
                {
                    Logger.Info("查询充电桩状态失败：超时！");
                    return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("查询充电桩状态失败！"));
            }
        }

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult GetChargingSummary(string userId)
        {
            return null;
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
