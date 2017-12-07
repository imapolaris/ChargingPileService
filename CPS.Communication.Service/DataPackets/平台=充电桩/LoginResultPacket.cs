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
        private LoginResultTypeEnum _result;
        /// <summary>
        /// 登录结果
        /// 1、登录成功；2、设备不存在；3、已经登录；4、密钥失效；5、其他错误
        /// </summary>
        public LoginResultTypeEnum ResultEnum
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

        public string ResultString
        {
            get
            {
                switch (this.ResultEnum)
                {
                    case LoginResultTypeEnum.Succeed:
                        return "登录成功";
                    case LoginResultTypeEnum.NotExists:
                        return "设备不存在";
                    case LoginResultTypeEnum.HasLogined:
                        return "已经登录";
                    case LoginResultTypeEnum.SecretKeyFailed:
                        return "密钥失效";
                    case LoginResultTypeEnum.Others:
                    default:
                        return "其他错误";
                }
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
            BodyLen = 5;
        }

        public bool HasLogined
        {
            get
            {
                return ResultEnum == LoginResultTypeEnum.Succeed || ResultEnum == LoginResultTypeEnum.HasLogined;
            }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            body[start] = this.Result;
            start += 1;
            byte[] temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);

            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._result = (LoginResultTypeEnum)buffer[start];
            start += 1;
            this._timestamp = BitConverter.ToInt32(buffer, start);

            return this;
        }
    }
}
