using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetSoftwareVerResultPacket : OperPacketBase
    {
        public GetSoftwareVerResultPacket() : base(PacketTypeEnum.GetSoftwareVerResult)
        {
            BodyLen = OperPacketBodyLen + 10;
        }

        private string _ver="";

        public string Ver
        {
            get { return _ver; }
            set { _ver = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = new byte[BodyLen];
            int start = OperPacketBodyLen;
            byte[] temp = EncodeHelper.GetBytes(this._ver);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._ver = EncodeHelper.GetString(buffer, start, 10);
            return this;
        }
    }
}
