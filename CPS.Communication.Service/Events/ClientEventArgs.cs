using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class ClientAcceptedEventArgs : EventArgs
    {
        private Client _client;

        public Client CurClient
        {
            get
            {
                return _client;
            }
        }

        public ClientAcceptedEventArgs(Client client)
        {
            this._client = client;
        }
    }

    public class ClientClosedEventArgs : EventArgs
    {
        public string Id { get; set; }

        public ClientClosedEventArgs(string id)
        {
            Id = id;
        }
    }
}
