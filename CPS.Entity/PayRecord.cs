using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class PayRecord : EntityBase
    {
        private string userId;
        private string payWay;
        private double payMoney;
        private string payDate;

        [StringLength(50)]
        [Required]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        [StringLength(10)]
        public string PayWay
        {
            get { return payWay; }
            set { payWay = value; }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public double PayMoney
        {
            get { return payMoney; }
            set { payMoney = value; }
        }

        /// <summary>
        /// 支付时间
        /// </summary>
        [StringLength(30)]
        public string PayDate
        {
            get { return payDate; }
            set { payDate = value; }
        }
    }
}
