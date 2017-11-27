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
        private PacketBase _packet;

        public ReceiveCompletedEventArgs(PacketBase packet)
        {
            _packet = packet;
        }

        public ReceiveCompletedEventArgs(PacketBase packet, Server.Client client)
            : this(packet)
        {
            _client = client;
        }

        public PacketBase ReceivedPacket { get { return _packet; } }

        private Server.Client _client;
        public Server.Client CurClient { get { return _client; } }
    }
}
