using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetReportIntervalPacket : OperPacketBase
    {
        public SetReportIntervalPacket() : base(PacketTypeEnum.SetReportInterval)
        {
            BodyLen = OperPacketBodyLen + 1 + 1;
        }

        public SetReportIntervalPacket(PacketTypeEnum pte) : base(pte)
        {

        }

        private byte _stateReportInterval;
        /// <summary>
        /// 状态数据上报间隔
        /// </summary>
        public byte StateReportInterval
        {
            get { return _stateReportInterval; }
            set { _stateReportInterval = value; }
        }

        private byte _realDataReportInterval;
        /// <summary>
        /// 实时数据上报间隔
        /// </summary>
        public byte RealDataReportInterval
        {
            get { return _realDataReportInterval; }
            set { _realDataReportInterval = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            body[start] = this._stateReportInterval;
            start += 1;
            body[start] = this._realDataReportInterval;
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._stateReportInterval = buffer[start];
            start += 1;
            this._realDataReportInterval = buffer[start];
            return this;
        }
    }
}
