using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.MessageSystem
{
    using Infrastructure.MQ;
    using RabbitMQ.Client;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    internal class MessageServer : IDisposable
    {
        [ImportMany]
        List<ServiceBase> Services;

        public MessageServer()
        {
            var aggCatalog = new AggregateCatalog(
                    new AssemblyCatalog(this.GetType().Assembly)
                );
            var container = new CompositionContainer(aggCatalog);
            container.ComposeParts(this);
        }

        public void Start()
        {
            
            if (Services == null || Services.Count <= 0)
                throw new ArgumentNullException("没有要启动的服务...");
            this.Services.AsParallel().ForAll(_ => _.StartMQConsumer());

            // 注册并启动短信服务
            SmsManager.Instance.Register();
        }

        public void Stop()
        {
            this.Services?.AsParallel().ForAll(_ => _.StopMQConsumer());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Services = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
