using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.State
{
    public class SendState
    {
        private Socket _socket;

        public Socket WorkSocket
        {
            get { return _socket; }
            set { _socket = value; }
        }
    }
}
