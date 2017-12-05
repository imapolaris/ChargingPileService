using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetSecretKeyPacket : OperPacketBase
    {
        public SetSecretKeyPacket() : base(PacketTypeEnum.SetSecretKey)
        {
            BodyLen = OperPacketBodyLen + 16 + 4;
        }

        public SetSecretKeyPacket(PacketTypeEnum pte) : base(pte) { }

        private string _secretKey;

        public string SecretKey
        {
            get { return _secretKey; }
            set { _secretKey = value; }
        }

        private int _timestamp;

        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = OperPacketBodyLen;
            byte[] temp = EncodeHelper.GetBytes(this._secretKey);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 16;
            temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = OperPacketBodyLen;
            this._secretKey = EncodeHelper.GetString(buffer, start, 16);
            start += 16;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
