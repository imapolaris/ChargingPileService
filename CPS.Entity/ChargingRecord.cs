using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CPS.Entities
{
    public class ChargingRecord : EntityBase
    {
        private string userId;
        private double kwhs;
        private string chargingDate;
        private double cost;

        /// <summary>
        /// 用户ID
        /// </summary>
        [StringLength(50)]
        [Required]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        /// <summary>
        /// 充电度数
        /// </summary>
        public double Kwhs
        {
            get { return kwhs; }
            set { kwhs = value; }
        }
        /// <summary>
        /// 充电日期
        /// </summary>
        [StringLength(30)]
        public string ChargingDate
        {
            get { return chargingDate; }
            set { chargingDate = value; }
        }
        /// <summary>
        /// 花费
        /// </summary>
        public double Cost
        {
            get { return cost; }
            set { cost = value; }
        }
    }
}