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

        private Client _client;
        public Client CurClient
        {
            get { return _client; }
            set { _client = value; }
        }

        public SendCompletedEventArgs(int len)
        {
            this._len = len;
        }

        public SendCompletedEventArgs(Client client, int len)
        {
            this._client = client;
            this._len = len;
        }
    }

    public class SendDataExceptionEventArgs : EventArgs
    {
        public SendDataExceptionEventArgs(Client client)
        {
            _client = client;
        }

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

        private Client _client;

        public Client CurClient
        {
            get { return _client; }
            set { _client = value; }
        }
    }
}
