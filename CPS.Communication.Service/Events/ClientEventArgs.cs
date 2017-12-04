using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public abstract class ClientEventArgs : EventArgs
    {
        private Client _client;

        public Client CurClient
        {
            get
            {
                return _client;
            }
        }

        public ClientEventArgs(Client client)
        {
            this._client = client;
        }
    }

    public class ClientAcceptedEventArgs : ClientEventArgs
    {
        public ClientAcceptedEventArgs(Client client)
            : base(client)
        {
        }
    }

    public class ClientDisconnectedEventArgs : ClientEventArgs
    {
        public ClientDisconnectedEventArgs(Client client)
            : base(client)
        { }
    }

    public class ClientClosedEventArgs : ClientEventArgs
    {
        public ClientClosedEventArgs(Client client)
            : base(client)
        {
            
        }
    }
}
