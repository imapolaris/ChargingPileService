using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public abstract class ClientEventArgs : EventArgs
    {
        private Server.Client _client;

        public Server.Client CurClient
        {
            get
            {
                return _client;
            }
        }

        public ClientEventArgs(Server.Client client)
        {
            this._client = client;
        }
    }

    public class ClientAcceptedEventArgs : ClientEventArgs
    {
        public ClientAcceptedEventArgs(Server.Client client)
            : base(client)
        {
        }
    }

    public class ClientClosedEventArgs : ClientEventArgs
    {
        public ClientClosedEventArgs(Server.Client client)
            : base(client)
        {
            
        }
    }
}
