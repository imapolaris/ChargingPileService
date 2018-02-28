using CPS.DB;
using Soaring.WebMonter.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ChargingPileService
{
    public abstract class OperatorBase : ApiController
    {
        protected bool IsDispose=false;
        protected CPS_Entities EntityContext;

        protected readonly SystemDbContext SysDbContext;
        protected readonly HistoryDbContext HisDbContext;

        public OperatorBase()
            : base()
        {
            EntityContext = new CPS_Entities();

            SysDbContext = new SystemDbContext();
            HisDbContext = new HistoryDbContext();
        }

        public new virtual void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        protected new virtual void Dispose(bool isDispose)
        {
            base.Dispose(isDispose);

            if (!IsDispose)
            {
                EntityContext.Dispose();
                IsDispose = true;
            }
        }
    }
}