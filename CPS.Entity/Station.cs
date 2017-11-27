using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CPS.Entities
{
    /// <summary>
    /// 电站
    /// </summary>
    [Serializable]
    public class Station : EntityBase
    {
        private string name;
        private double longitude;
        private double latitude;
        private string numbers;
        private string address;
        private double distance;

        private string openingHours;
        private short payWay;
        private long chargingTimes;
        private string briefIntroduce;
        private byte[][] introduceImages;

        private float elecPrice;
        private float servicePrice;

        [StringLength(50)]
        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        [Required]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        /// <summary>
        /// 电桩数量
        /// </summary>
        [StringLength(30)]
        public string Numbers
        {
            get { return numbers; }
            set { numbers = value; }
        }

        [StringLength(200)]
        [Required]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        [NotMapped]
        public double Distance
        {
            get { return distance; }
            set { distance = Math.Round(value, 2); }
        }

        /// <summary>
        /// 营业时间
        /// </summary>
        [StringLength(30)]
        public string OpeningHours
        {
            get { return openingHours; }
            set { openingHours = value; }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public short PayWay
        {
            get { return payWay; }
            set { payWay = value; }
        }

        public PayWayEnum PayWayType
        {
            get
            {
                return (PayWayEnum)this.payWay;
            }
        }

        public string PayWayDesc
        {
            get
            {
                string desc = "";
                switch (this.payWay)
                {
                    case 0x00:
                        desc = "全部";
                        break;
                    case 0x01:
                        desc = "支付宝";
                        break;
                    case 0x02:
                        desc = "微信";
                        break;
                    case 0x03:
                        desc = "支付宝，微信";
                        break;
                    case 0x04:
                        desc = "现金";
                        break;
                    case 0x07:
                        desc = "支付宝，微信，现金";
                        break;
                    case 0x08:
                        desc = "未知";
                        break;
                    default:
                        break;
                }

                return desc;
            }
        }

        /// <summary>
        /// 累计充电次数
        /// </summary>
        public long ChargingTimes
        {
            get { return chargingTimes; }
            set { chargingTimes = value; }
        }

        /// <summary>
        /// 简要介绍
        /// </summary>
        public string BriefIntroduce
        {
            get { return briefIntroduce; }
            set { briefIntroduce = value; }
        }

        /// <summary>
        /// 现场图片
        /// </summary>
        public byte[][] IntroduceImages
        {
            get { return introduceImages; }
            set { introduceImages = value; }
        }

        /// <summary>
        /// 电价
        /// </summary>
        [NotMapped]
        public float ElecPrice
        {
            get { return elecPrice; }
            set { elecPrice = value; }
        }

        /// <summary>
        /// 服务费
        /// </summary>
        [NotMapped]
        public float ServicePrice
        {
            get { return servicePrice; }
            set { servicePrice = value; }
        }
    }
}