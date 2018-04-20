using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class SetTimePeriodPacket : OperPacketBase
    {
        public SetTimePeriodPacket() : base(PacketTypeEnum.SetTimePeriod)
        {

        }

        private byte _numberOfSharpPeriod;

        public byte NumberOfSharpPeriod
        {
            get { return _numberOfSharpPeriod; }
            set { _numberOfSharpPeriod = value; }
        }

        private byte[] _sharpPeriods;

        public byte[] SharpPeriods
        {
            get { return _sharpPeriods; }
            set { _sharpPeriods = value; }
        }

        private byte _numberOfPeakPeriod;

        public byte NumberOfPeakPeriod
        {
            get { return _numberOfPeakPeriod; }
            set { _numberOfPeakPeriod = value; }
        }

        private byte[] _peakPeriods;

        public byte[] PeakPeriods
        {
            get { return _peakPeriods; }
            set { _peakPeriods = value; }
        }

        private byte _numberOfFlatPeriod;

        public byte NumberOfFlatPeriod
        {
            get { return _numberOfFlatPeriod; }
            set { _numberOfFlatPeriod = value; }
        }

        private byte[] _flatPeriods;

        public byte[] FlatPeriods
        {
            get { return _flatPeriods; }
            set { _flatPeriods = value; }
        }

        private byte _numberOfValleyPeriod;

        public byte NumberOfValleyPeriod
        {
            get { return _numberOfValleyPeriod; }
            set { _numberOfValleyPeriod = value; }
        }

        private byte[] _valleyPeriods;

        public byte[] ValleyPeriods
        {
            get { return _valleyPeriods; }
            set { _valleyPeriods = value; }
        }

        public override byte[] EncodeBody()
        {
            BodyLen = OperPacketBodyLen + 1 + this._numberOfSharpPeriod * 2 + 1 + this._numberOfPeakPeriod * 2 + 1 + this._numberOfFlatPeriod * 2 + 1 + this._numberOfValleyPeriod * 2;

            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            body[start] = this._numberOfSharpPeriod;
            start += 1;
            for (int i = 0; i < this._sharpPeriods.Length; i++)
            {
                body[start] = this._sharpPeriods[i];
                start += 1;
            }
            body[start] = this._numberOfPeakPeriod;
            start += 1;
            for (int i = 0; i < this._peakPeriods.Length; i++)
            {
                body[start] = this._peakPeriods[i];
                start += 1;
            }
            body[start] = this._numberOfFlatPeriod;
            start += 1;
            for (int i = 0; i < this._flatPeriods.Length; i++)
            {
                body[start] = this._flatPeriods[i];
                start += 1;
            }
            body[start] = this._numberOfValleyPeriod;
            start += 1;
            for (int i = 0; i < this._valleyPeriods.Length; i++)
            {
                body[start] = this._valleyPeriods[i];
                start += 1;
            }

            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._numberOfSharpPeriod = buffer[start];
            start += 1;
            this._sharpPeriods = new byte[this._numberOfSharpPeriod * 2];
            for (int i = 0; i < this._sharpPeriods.Length; i++)
            {
                this._sharpPeriods[i] = buffer[start];
                start += 1;
            }
            this._numberOfPeakPeriod = buffer[start];
            start += 1;
            this._peakPeriods = new byte[this._numberOfPeakPeriod * 2];
            for (int i = 0; i < this._peakPeriods.Length; i++)
            {
                this._peakPeriods[i] = buffer[start];
                start += 1;
            }
            this._numberOfFlatPeriod = buffer[start];
            start += 1;
            this._flatPeriods = new byte[this._numberOfFlatPeriod * 2];
            for (int i = 0; i < this._flatPeriods.Length; i++)
            {
                this._flatPeriods[i] = buffer[start];
                start += 1;
            }
            this._numberOfValleyPeriod = buffer[start];
            start += 1;
            this._valleyPeriods = new byte[this._numberOfValleyPeriod * 2];
            for (int i = 0; i < this._valleyPeriods.Length; i++)
            {
                this._valleyPeriods[i] = buffer[start];
                start += 1;
            }

            return this;
        }
    }
}
