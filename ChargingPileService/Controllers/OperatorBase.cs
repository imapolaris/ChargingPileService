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
        protected readonly SystemDbContext SysDbContext;
        protected readonly HistoryDbContext HisDbContext;

        public OperatorBase()
            : base()
        {
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
                SysDbContext.Dispose();
                HisDbContext.Dispose();
                IsDispose = true;
            }
        }
    }
}