using CPS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public interface IUniversal
    {
        UniversalData GetUniversalData();
    }
}
