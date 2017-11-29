using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class LoginResultPacket : PacketBase
    {
        private LoginResultEnum _result;
        /// <summary>
        /// 登录结果
        /// 1、登录成功；2、设备不存在；3、已经登录；4、密钥失效；5、其他错误
        /// </summary>
        public LoginResultEnum ResultEnum
        {
            get { return _result; }
            set { _result = value; }
        }

        public byte Result
        {
            get
            {
                return (byte)_result;
            }
        }

        private int _timestamp;

        /// <summary>
        /// 当前时间
        /// </summary>
        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public DateTime CurDate
        {
            get
            {
                return DateHelper.ConvertToDateX(this._timestamp);
            }
        }

        public LoginResultPacket() : base(PacketTypeEnum.LoginResult)
        {
            BodyLen = PacketBase.SerialNumberLen + 5;
        }

        public bool HasLogined
        {
            get
            {
                return ResultEnum == LoginResultEnum.Succeed || ResultEnum == LoginResultEnum.HasLogined;
            }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = SerialNumberLen;
            body[start] = this.Result;
            start += 1;
            byte[] temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);

            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = SerialNumberLen;
            this._result = (LoginResultEnum)buffer[start];
            start += 1;
            this._timestamp = BitConverter.ToInt32(buffer, start);

            return this;
        }
    }
}
