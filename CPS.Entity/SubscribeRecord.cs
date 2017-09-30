using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CPS.Entities
{
    public class SubscribeRecord : EntityBase
    {
        private string userId;
        private string serialNumber;
        private string subscribeDate;
        private string subscribeStatus;

        [StringLength(50)]
        [Required]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        /// <summary>
        /// 电桩编号
        /// </summary>
        [StringLength(30)]
        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        /// <summary>
        /// 预约时间
        /// </summary>
        [StringLength(30)]
        public string SubscribeDate
        {
            get { return subscribeDate; }
            set { subscribeDate = value; }
        }

        /// <summary>
        /// 预约结果
        /// </summary>
        [StringLength(10)]
        public string SubscribeStatus
        {
            get { return subscribeStatus; }
            set { subscribeStatus = value; }
        }
    }
}