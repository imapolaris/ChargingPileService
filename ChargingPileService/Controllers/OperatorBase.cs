using CPS.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ChargingPileService
{
    public abstract class OperatorBase : ApiController
    {
        protected bool IsDispose;
        protected CPS_Entities EntityContext;
        public OperatorBase()
            : base()
        {
            EntityContext = new CPS_Entities();
        }

        public new void Dispose()
        {
            base.Dispose();
            Dispose(IsDispose);
        }
        protected new virtual void Dispose(bool isDispose)
        {
            base.Dispose(isDispose);

            if (!isDispose)
            {
                EntityContext.Dispose();
                GC.SuppressFinalize(this);
                IsDispose = true;
            }
        }
    }
}