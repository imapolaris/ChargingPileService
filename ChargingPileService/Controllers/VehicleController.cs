using ChargingPileService.Models;
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
    [RoutePrefix("api/vehicle")]
    public class VehicleController : OperatorBase
    {
        [HttpGet]
        public IEnumerable<VehicleInfo> GetVehicleInfoes(string userId)
        {
            try
            {
                return SysDbContext.PlageCustomers.Where(_ => _.Customerd == userId).Select(_ => _.Vehicle);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        [HttpPost]
        [Route("submit")]
        public IHttpActionResult AddOneVehicle(dynamic obj)
        {
            try
            {
                string userId = obj.userId;
                string models = obj.models;
                string plateno = obj.plateno;

                var vehicle = new VehicleInfo()
                {
                    Models = models,
                    PlateNo = plateno,
                };
                SysDbContext.VehicleInfoes.Add(vehicle);

                SysDbContext.PlageCustomers.Add(new PlageCustomer()
                {
                    Customerd = userId,
                    VehicleId = vehicle.Id,
                });
                SysDbContext.SaveChanges();
                return Ok(Models.SingleResult<VehicleInfo>.Succeed("添加成功!", vehicle));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(SimpleResult.Failed("添加失败！"));
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult DeleteOneVehicle(dynamic obj)
        {
            string vehicleId = obj.vehicleId;

            var result = SysDbContext.VehicleInfoes.Where(_ => _.Id == vehicleId);
            if (result != null && result.Count() > 0)
            {
                SysDbContext.VehicleInfoes.RemoveRange(result);
                SysDbContext.SaveChanges();
                return Ok(SimpleResult.Succeed("删除成功！"));
            }
            else
            {
                return Ok(SimpleResult.Failed("删除失败！"));
            }
        }
    }
}
