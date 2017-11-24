using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class ChangeSecretKeyPacket : OperBasePacket
    {
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


    }
}
