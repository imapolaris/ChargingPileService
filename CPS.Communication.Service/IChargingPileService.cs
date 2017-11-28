using CPS.Communication.Service.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    public interface IChargingPileService
    {
        bool startCharging(string sn);
        object getChargingStatus(string sn);
        bool stopCharging(string sn);

        void ServiceFactory(object sender, ReceiveCompletedEventArgs args);
    }
}
