using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem.Jobs
{
    using Quartz;
    using CPS.Infrastructure.Utils;

    /// <summary>
    /// 预约任务
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class SubscribeJob : JobBase, IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            try
            {
                await Console.Error.WriteLineAsync("Hello");
            }
            catch (JobExecutionException ex)
            {
                ex.RefireImmediately = true;
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
