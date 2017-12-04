﻿using CPS.Communication.Service.DataPackets;
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
        Server MyServer { get; set; }

        bool startCharging(string sn);
        object getChargingStatus(string sn);
        bool stopCharging(string sn);

        void ServiceFactory(Client client, PacketBase packet);
    }
}