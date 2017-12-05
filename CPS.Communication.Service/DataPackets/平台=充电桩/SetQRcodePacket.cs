using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetQRcodePacket : OperPacketBase
    {
        public SetQRcodePacket() : base(PacketTypeEnum.SetQRcode)
        {
            // 二维码长度是变动的
            // BodyLen = OperPacketBodyLen + 1 + 1 + 1 + x;
        }

        public SetQRcodePacket(PacketTypeEnum pte) : base(pte)
        {

        }

        private byte _qnumbers;
        /// <summary>
        /// 枪口个数
        /// </summary>
        public byte QNumbers
        {
            get { return _qnumbers; }
            set { _qnumbers = value; }
        }

        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        private byte _qrcodeLen;
        /// <summary>
        /// 二维码长度
        /// </summary>
        public byte QRcodeLen
        {
            get { return _qrcodeLen; }
            set { _qrcodeLen = value; }
        }

        private string _qrcode;
        /// <summary>
        /// 二维码
        /// </summary>
        public string QRcode
        {
            get { return _qrcode; }
            set { _qrcode = value; }
        }

        public override byte[] EncodeBody()
        {
            BodyLen = OperPacketBodyLen + 1 + 1 + 1 + this._qrcodeLen;
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            body[start] = this._qnumbers;
            start += 1;
            body[start] = this._qport;
            start += 1;
            body[start] = this._qrcodeLen;
            start += 1;
            byte[] temp = EncodeHelper.GetBytes(this._qrcode);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._qnumbers = buffer[start];
            start += 1;
            this._qport = buffer[start];
            start += 1;
            this._qrcodeLen = buffer[start];
            start += 1;
            this._qrcode = EncodeHelper.GetString(buffer, start, this._qrcodeLen);
            return this;
        }
    }
}
