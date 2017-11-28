using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class ReceiveCompletedEventArgs : EventArgs
    {
        private byte[] _buffer;
        public ReceiveCompletedEventArgs(byte[] bytes)
        {
            _buffer = bytes;
        }

        public byte[] ReceivedBytes { get { return _buffer; } }

        private string _rStr;
        public string ReceivedAsString
        {
            get
            {
                if (_rStr == null && ReceivedBytes != null)
                    _rStr = Encoding.UTF8.GetString(ReceivedBytes);
                return _rStr;
            }
        }

        public int ByteLength { get { return _buffer == null ? 0 : _buffer.Length; } }
    }
}
