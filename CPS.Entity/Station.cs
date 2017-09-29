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
    }
}