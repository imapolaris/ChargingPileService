using CPS.Infrastructure.Utils;
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
        private string gender = "男";
        private string password;
        private string newPassword;
        private string phoneNumber;
        private string avatar;
        private string vcode;

        private string username;
        private string shippingAddress;
        private string familyAddress;
        private string memberLevel;
        private string carLicense;
        private string carType;
        private string registrationId;

        private byte userCategory;

        private int subscribeTimesToday;

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
        /// 新密码
        /// </summary>
        [StringLength(100)]
        [NotMapped]
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
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
            set { vcode = value; }
        }

        /// <summary>
        /// 账号
        /// </summary>
        [StringLength(100)]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// 收货地址
        /// </summary>
        [StringLength(255)]
        public string ShippingAddress
        {
            get { return shippingAddress; }
            set { shippingAddress = value; }
        }

        /// <summary>
        /// 家庭地址
        /// </summary>
        [StringLength(255)]
        public string FamilyAddress
        {
            get { return familyAddress; }
            set { familyAddress = value; }
        }

        /// <summary>
        /// 会员等级
        /// </summary>
        [StringLength(10)]
        public string MemberLevel
        {
            get { return memberLevel; }
            set { memberLevel = value; }
        }

        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(10)]
        public string CarLicense
        {
            get { return carLicense; }
            set { carLicense = value; }
        }

        /// <summary>
        /// 车型号
        /// </summary>
        [StringLength(15)]
        public string CarType
        {
            get { return carType; }
            set { carType = value; }
        }

        /// <summary>
        /// 今天预约次数
        /// </summary>
        [NotMapped]
        public int SubscribeTimesToday
        {
            get { return subscribeTimesToday; }
            set { subscribeTimesToday = value; }
        }

        /// <summary>
        /// 今天是否可以再预约
        /// </summary>
        [NotMapped]
        public bool CanSubscribeToday
        {
            get
            {
                int times = 3;
                try
                {
                    times = int.Parse(ConfigHelper.GetValue(typeof(User).Assembly, "reservationTimes"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return this.subscribeTimesToday <= times;
            }
        }


        [NotMapped]
        public string RegistrationId
        {
            get { return registrationId; }
            set { registrationId = value; }
        }

        /// <summary>
        /// 用户类别
        /// </summary>
        public byte UserCategory
        {
            get { return userCategory; }
            set { userCategory = value; }
        }

        public UserTypeEnum UserCategoryEmum
        {
            get
            {
                return (UserTypeEnum)this.userCategory;
            }
        }
    }
}
