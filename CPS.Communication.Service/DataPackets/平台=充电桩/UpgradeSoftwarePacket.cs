using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class UpgradeSoftwarePacket : OperPacketBase
    {
        public UpgradeSoftwarePacket() : base(PacketTypeEnum.UpgradeSoftware)
        {
            BodyLen = OperPacketBodyLen + 10 + 1 /*+x*/ + 8 + 8 + 10;
        }

        private string _verNo;
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string VerNo
        {
            get { return _verNo; }
            set { _verNo = value; }
        }

        private byte _ftpAddressLen;
        /// <summary>
        /// FTP地址长度
        /// </summary>
        public byte FtpAddressLen
        {
            get { return _ftpAddressLen; }
            set { _ftpAddressLen = value; }
        }

        private string _ftpAddress;
        /// <summary>
        /// FTP地址
        /// </summary>
        public string FtpAddress
        {
            get { return _ftpAddress; }
            set { _ftpAddress = value; }
        }

        private string _ftpUsername;
        /// <summary>
        /// FTP账号
        /// </summary>
        public string FtpUsername
        {
            get { return _ftpUsername; }
            set { _ftpUsername = value; }
        }

        private string _ftpPwd;
        /// <summary>
        /// FTP密码
        /// </summary>
        public string FtpPwd
        {
            get { return _ftpPwd; }
            set { _ftpPwd = value; }
        }

        private string _softwareName;
        /// <summary>
        /// 软件包名称
        /// </summary>
        public string SoftwareName
        {
            get { return _softwareName; }
            set { _softwareName = value; }
        }

        public override byte[] EncodeBody()
        {
            BodyLen += this._ftpAddressLen;
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            byte[] temp = EncodeHelper.GetBytes(this._verNo);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 10;
            body[start] = this._ftpAddressLen;
            start += 1;
            temp = EncodeHelper.GetBytes(this._ftpAddress);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += this._ftpAddressLen;
            temp = EncodeHelper.GetBytes(this._ftpUsername);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            temp = EncodeHelper.GetBytes(this._ftpPwd);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            temp = EncodeHelper.GetBytes(this._softwareName);
            Array.Copy(temp, 0, body, start, temp.Length);
            return base.EncodeBody();
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._verNo = EncodeHelper.GetString(buffer, start, 10);
            start += 10;
            this._ftpAddressLen = buffer[start];
            start += 1;
            this._ftpAddress = EncodeHelper.GetString(buffer, start, this._ftpAddressLen);
            start += this._ftpAddressLen;
            this._ftpUsername = EncodeHelper.GetString(buffer, start, 8);
            start += 8;
            this._ftpPwd = EncodeHelper.GetString(buffer, start, 8);
            start += 8;
            this._softwareName = EncodeHelper.GetString(buffer, start, 10);
            return this;
        }
    }
}
