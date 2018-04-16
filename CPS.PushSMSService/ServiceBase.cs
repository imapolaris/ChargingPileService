using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.MessageSystem
{
    using RabbitMQ.Client;
    using CPS.Infrastructure.MQ;
    using System.ComponentModel.Composition;

    [InheritedExport(typeof(ServiceBase))]
    public class ServiceBase : IDisposable
    {
        protected ConnectionFactory factory;
        protected IConnection connection;
        protected IModel channel;

        protected virtual string QueueName { get; set; }

        public ServiceBase()
        {
            var hostname = MqConfiguration.GetConfig().HostName;
            factory = new ConnectionFactory() { HostName = hostname };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public virtual void StartMQConsumer()
        {
            //Console.WriteLine("start one mq consumer...");
        }

        public virtual void StopMQConsumer()
        {
            //Console.WriteLine("stop the mq consumer...");
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

                channel?.Close();
                channel?.Dispose();
                channel = null;

                connection?.Close();
                connection?.Dispose();
                connection = null;

                factory = null;

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
