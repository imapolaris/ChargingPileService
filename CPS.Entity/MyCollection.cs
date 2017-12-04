using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class MyCollection : EntityBase
    {
        private string userId;

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private string stationId;

        public string StationId
        {
            get { return stationId; }
            set { stationId = value; }
        }

    }
}
