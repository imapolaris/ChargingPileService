using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class User : EntityBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(20)]
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20)]
        public string NickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DefaultValue("男")]
        [StringLength(1)]
        public string Gender { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(100)]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        [StringLength(100)]
        [JsonIgnore]
        public string Address { get; set; }
    }
}
