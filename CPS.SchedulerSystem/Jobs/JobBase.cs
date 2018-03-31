using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem.Jobs
{
    public class JobBase
    {
        public JobBase()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}
