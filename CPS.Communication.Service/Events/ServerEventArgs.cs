using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class ServerStartedEventArgs : EventArgs
    {
        private string _msg;

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

    }

    public class ServerStoppedEventArgs : EventArgs
    {
        private string _msg;

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }
}
