using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.Scheduler
{
    public class JobScheduler_beta
    {
        public static StdSchedulerFactory SchedulerFactory;
        public static IScheduler Scheduler;
        public static void start()
        {
            try
            {
                Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

                SchedulerFactory = new StdSchedulerFactory();
                //Grab the scheduler instance from the factory
                Scheduler = SchedulerFactory.GetScheduler();

                // and start it off
                Scheduler.Start();

                //define the job and tie it to our ProcessFirstTimeInventoryReportsJob Class   
                IJobDetail ReportsForNewUserJob = JobBuilder.Create<ProcessFirstTimeInventoryReportsJob>()
                                 .WithIdentity("Job1", "group1")
                                    .Build();
                //Trigger the job to run
                ITrigger ReportsForNewUser_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger1", "group1")
                     .WithCronSchedule("0 2/10 3-23 1/1 * ? *")// Run Every 10 minutes from 03:02 AM
                     .ForJob(ReportsForNewUserJob)
                     .Build();

                //define the job and tie it to our ProcessSnapShotReportsJob Class   
                IJobDetail SnapShotReportsJob = JobBuilder.Create<ProcessSnapShotReportsJob>()
                                 .WithIdentity("Job2", "group2")
                                    .Build();
                ITrigger SnapShotReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger2", "group2")
                     .WithCronSchedule("0 0/15 3-23 1/1 * ? *")// Runs Every 15 minutes from 03:00 AM
                     .ForJob(SnapShotReportsJob)
                     .Build();

                //define the job and tie it to our ProcessInventoryReportsJob Class   
                IJobDetail InventoryReportsJob = JobBuilder.Create<ProcessInventoryReportsJob>()
                                 .WithIdentity("Job3", "group3")
                                    .Build();
                //Trigger the job to run              
                ITrigger InventoryReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger3", "group3")
                     .WithCronSchedule("0 1/20 3-23 1/1 * ? *")// Runs Every 20 minutes from 03:01 AM
                     .ForJob(InventoryReportsJob)
                     .Build();

                //define the job and tie it to our ProcessRefreshReportsJob Class   
                IJobDetail GetRefreshReportsJob = JobBuilder.Create<ProcessRefreshReportsJob>()
                                 .WithIdentity("Job4", "group4")
                                    .Build();
                //Trigger the job to run              
                ITrigger GetRefreshReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger4", "group4")
                     .WithCronSchedule("0 0/25 7-19 1/1 * ? *")// Runs Every 25 minutes from 07:00 AM - 07:57 PM
                     .ForJob(GetRefreshReportsJob)
                     .Build();

                //define the job and tie it to our ProcessFailedReportsJob Class   
                IJobDetail GetFailedReportsJob = JobBuilder.Create<ProcessFailedReportsJob>()
                                 .WithIdentity("Job5", "group5")
                                    .Build();
                //Trigger the job to run              
                ITrigger GetFailedReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger5", "group5")
                     .WithCronSchedule("0 10/30 7-19 1/1 * ? *")// Runs Every 30 minutes from 07:10 AM - 07:50 PM
                     .ForJob(GetFailedReportsJob)
                     .Build();

                //define the job and tie it to our ProcessOptimizations Class
                IJobDetail OptimizationsJob = JobBuilder.Create<ProcessOptimizationsJob>()
                                 .WithIdentity("Job6", "group6")
                                    .Build();
                //Trigger the job to run
                ITrigger Optimizations_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger6", "group6")
                     .WithCronSchedule("0 0/5 23-23 1/1 * ? *")// Runs every 5 minutes from 11:00 PM
                     .ForJob(OptimizationsJob)
                     .Build();

                //define the job and tie it to our Process_AllservicesJob Class
                IJobDetail AllservicesJob = JobBuilder.Create<Process_AllservicesJob>()
                                 .WithIdentity("Job7", "group7")
                                    .Build();
                //Trigger the job to run
                ITrigger Allservices_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger7", "group7")
                     .WithCronSchedule("0 0/5 * 1/1 * ? *")// Runs Every 5 minutes
                     .ForJob(AllservicesJob)
                     .Build();

                //define the job and tie it to our Process_DailyServicesJob Class   
                IJobDetail DailyServicesJob = JobBuilder.Create<Process_DailyServicesJob>()
                                 .WithIdentity("Job8", "group8")
                                    .Build();
                //Trigger the job to run              
                ITrigger DailyServices_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger8", "group8")
                     .WithCronSchedule("0 30 0 1/1 * ? *")// Runs at 12:30 AM Every day
                     .ForJob(DailyServicesJob)
                     .Build();


                //define the job and tie it to our Process_DailyServicesJob Class   
                IJobDetail Process_UserArchieveData = JobBuilder.Create<Process_AnalyticsUsersCount>()
                                 .WithIdentity("Job9", "group9")
                                    .Build();
                //Trigger the job to run              
                ITrigger Process_UserArchieveData_Trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger9", "group9")
                     .WithCronSchedule("0 0/2 * 1/1 * ? *")// Runs at 12:30 AM Every day
                     .ForJob(Process_UserArchieveData)
                     .Build();

                //Tell quartz to schedule the job using trigger
                Scheduler.ScheduleJob(ReportsForNewUserJob, ReportsForNewUser_trigger);
                Scheduler.ScheduleJob(SnapShotReportsJob, SnapShotReports_trigger);
                Scheduler.ScheduleJob(InventoryReportsJob, InventoryReports_trigger);
                Scheduler.ScheduleJob(GetRefreshReportsJob, GetRefreshReports_trigger);
                Scheduler.ScheduleJob(GetFailedReportsJob, GetFailedReports_trigger);
                Scheduler.ScheduleJob(OptimizationsJob, Optimizations_trigger);
                Scheduler.ScheduleJob(AllservicesJob, Allservices_trigger);
                Scheduler.ScheduleJob(DailyServicesJob, DailyServices_trigger);
                Scheduler.ScheduleJob(Process_UserArchieveData, Process_UserArchieveData_Trigger);
                //scheduler.Shutdown();
            }
            catch (Exception err)
            {
                throw new Quartz.JobExecutionException(err);
            }
        }
    }
}