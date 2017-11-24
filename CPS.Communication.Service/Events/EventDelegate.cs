using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public delegate void ReceiveCompletedHandler(object sender, ReceiveCompletedEventArgs args);
    public delegate void SendCompletedHandler(object sender, SendCompletedEventArgs args);
    public delegate void ErrorOccurredHandler(object sender, ErrorEventArgs args);
    public delegate void ClientAcceptedHandler(object sender, ClientAcceptedEventArgs args);
    public delegate void ClientClosedHandler(object sender, ClientClosedEventArgs args);

    public delegate void ServerStartedHandler(object sender, ServerStartedEventArgs args);
    public delegate void ServerStoppedHandler(object sender, ServerStoppedEventArgs args);
}
