using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    [Serializable]
    public class User : EntityBase
    {
        private string nickname;
        private string gender;
        private string password;
        private string phoneNumber;
        private string avatar;
        private string vcode;

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20)]
        public string NickName
        {
            get { return nickname; }
            set { nickname = value; }
        }
        /// <summary>
        /// 性别
        /// </summary>
        [DefaultValue("男")]
        [StringLength(1)]
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(100)]
        [Required]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11)]
        [Required]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }
        /// <summary>
        /// 验证码
        /// </summary>
        [NotMapped]
        public string VCode
        {
            get { return vcode; }
        }
    }
}
