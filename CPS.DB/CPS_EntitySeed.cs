using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DB
{
    public class EMNMS_EntitySeed : DropCreateDatabaseIfModelChanges<CPS_Entities>
    {
        protected override void Seed(CPS_Entities context)
        {
            SeedConfig.Seed(context);

            base.Seed(context);
        }
    }
}
