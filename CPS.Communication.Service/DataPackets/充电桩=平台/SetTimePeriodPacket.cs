using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class SetTimePeriodPacket : OperBasePacket
    {
        private byte _numberOfSharpPeriod;

        public byte NumberOfSharpPeriod
        {
            get { return _numberOfSharpPeriod; }
            set { _numberOfSharpPeriod = value; }
        }

        private List<byte[]> _sharpPeriods;

        public List<byte[]> SharpPeriods
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

        private List<byte[]> _peakPeriods;

        public List<byte[]> PeakPeriods
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

        private List<byte[]> _flatPeriods;

        public List<byte[]> FlatPeriods
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

        private List<byte[]> _valleyPeriods;

        public List<byte[]> ValleyPeriods
        {
            get { return _valleyPeriods; }
            set { _valleyPeriods = value; }
        }
    }
}
