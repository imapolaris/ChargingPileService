using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem
{
    using Quartz;
    using CPS.SchedulerSystem.Common;
    using System.Collections.Specialized;
    using System.Configuration;
    using Quartz.Impl;

    /// <summary>
    /// 任务管理器
    /// </summary>
    public class JobManager
    {
        private ISchedulerFactory SchedulerFactory { get; set; } = null;

        #region 【singleton】

        private JobManager()
        {

        }

        private static JobManager _instance = null;

        public static JobManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new JobManager();
                }

                return _instance;
            }
        }

        #endregion

        private ISchedulerFactory GetSchedulerFactory()
        {
            var properties = JobSchedulerConfig();
            if (SchedulerFactory == null)
                SchedulerFactory = new StdSchedulerFactory(properties);

            return SchedulerFactory;
        }

        public IScheduler GetScheduler(string schedName)
        {
            return GetSchedulerFactory().GetScheduler();
        }

        public IJob CreateJob(JobTypeEnum jobType)
        {
            switch (jobType)
            {
                case JobTypeEnum.Subscribe:
                    break;
                default:
                    break;
            }


            return null;
        }

        public ITrigger CreateTrigger()
        {
            return null;
        }

        #region 【配置】

        ////===调度器配置===
        private NameValueCollection JobSchedulerConfig()
        {
            var properties = new NameValueCollection();

            properties["quartz.scheduler.instanceName"] = "SubscribeJobScheduler";

            ThreadPoolConfig(properties);
            JobPersistenceConfig(properties);
            RemoteExporterConfig(properties);
            ClusterConfig(properties);

            return properties;    
        }


        ////===持久化配置===
        private NameValueCollection JobPersistenceConfig(NameValueCollection properties)
        {
            if(properties == null)
            {
                properties = new NameValueCollection();
            }

            //存储类型
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //数据源名称
            properties["quartz.jobStore.dataSource"] = "myDS";
            //表明前缀
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            //properties["quartz.jobStore.useProperties"] = "false";
            //properties["quartz.serializer.type"] = "binary";
            //驱动类型
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
            //连接字符串
            properties["quartz.dataSource.myDS.connectionString"] = ConfigurationManager.ConnectionStrings["jobSchedulerConnection"].ConnectionString;
            //sqlserver版本
            properties["quartz.dataSource.myDS.provider"] = "SqlServer-20";

            return properties;
        }

        ////===线程池配置===
        private NameValueCollection ThreadPoolConfig(NameValueCollection properties)
        {
            if (properties == null)
            {
                properties = new NameValueCollection();
            }

            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "50";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            return properties;
        }

        ////===远程输出配置===
        private NameValueCollection RemoteExporterConfig(NameValueCollection properties)
        {
            if (properties == null)
            {
                properties = new NameValueCollection();
            }

            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
            properties["quartz.scheduler.exporter.channelType"] = "tcp";

            return properties;
        }

        ////===集群===
        private NameValueCollection ClusterConfig(NameValueCollection properties)
        {
            if (properties == null)
            {
                properties = new NameValueCollection();
            }

            properties["quartz.jobStore.clustered"] = "true";
            properties["quartz.scheduler.instanceId"] = "AUTO";

            return properties;
        }

        #endregion 【配置】
    }
}
