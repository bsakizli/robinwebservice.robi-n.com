using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;

namespace RobinOperasyonWebService.Scheduling
{

   
    public class SchedulerWrapper : BaseDAL
    {
       
        public void RunJob()
        {



            try
            {
                
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                IScheduler sched = schedFact.GetScheduler().GetAwaiter().GetResult();
                if (!sched.IsStarted)
                    sched.Start();

                IJobDetail jobX = JobBuilder.Create<XScheduler>().WithIdentity("XScheduler", null).UsingJobData("SchedulerId", "123").Build();
                ISimpleTrigger triggerX = (ISimpleTrigger)TriggerBuilder.Create().WithIdentity("XScheduler").StartAt(DateTime.UtcNow).WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever()).Build();
                sched.ScheduleJob(jobX, triggerX);

            }
            catch (Exception ex)
            {
            }
        }






    }
}