using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.MQ
{
    public interface IMqManager
    {
        void Start();
        void Stop();

        event MessageReceivedHandler MessageReceived;
    }
}
