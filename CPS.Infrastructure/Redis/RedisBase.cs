using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Redis
{
    using StackExchange.Redis;

    public abstract class RedisBase : IDisposable
    {
        protected virtual ConnectionMultiplexer Client { get; private set; }
        private bool _disposed = false;
        protected RedisBase()
        {
            Client = RedisManager.GetClient();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Client.Dispose();
                    Client = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
