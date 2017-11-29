using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class SendCompletedEventArgs : EventArgs
    {
        private int _len;

        public int Len
        {
            get { return _len; }
            set { _len = value; }
        }

        public SendCompletedEventArgs()
        {
        }

        public SendCompletedEventArgs(int len)
        {
            this._len = len;
        }
    }

    public class SendDataExceptionEventArgs : EventArgs
    {
        private Exception _innerException;

        public Exception InnerException
        {
            get { return _innerException; }
            set { _innerException = value; }
        }

        private string _msg;

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        private Server.Client _client;

        public Server.Client CurClient
        {
            get { return _client; }
            set { _client = value; }
        }
    }
}
