using Quartz;
using Quartz.Impl;
using System;

namespace Mpp.BUSINESS.Scheduler
{
    public class JobScheduler
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
                     .WithCronSchedule("0 2/3 0-23 1/1 * ? *")// Run Every 3 minutes from 00:02 AM
                     .ForJob(ReportsForNewUserJob)
                     .Build();

                //define the job and tie it to our ProcessSnapShotReportsJob Class   
                IJobDetail SnapShotReportsJob = JobBuilder.Create<ProcessSnapShotReportsJob>()
                                 .WithIdentity("Job2", "group2")
                                    .Build();
                ITrigger SnapShotReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger2", "group2")
                     .WithCronSchedule("0 0/3 0-23 1/1 * ? *")// Runs Every 3 minutes from 00:00 AM
                     .ForJob(SnapShotReportsJob)
                     .Build();

                //define the job and tie it to our ProcessInventoryReportsJob Class   
                IJobDetail InventoryReportsJob = JobBuilder.Create<ProcessInventoryReportsJob>()
                                 .WithIdentity("Job3", "group3")
                                    .Build();
                //Trigger the job to run              
                ITrigger InventoryReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger3", "group3")
                     .WithCronSchedule("0 1/3 0-23 1/1 * ? *")// Runs Every 3 minutes from 00:01 AM
                     .ForJob(InventoryReportsJob)
                     .Build();

                //define the job and tie it to our ProcessRefreshReportsJob Class   
                IJobDetail GetRefreshReportsJob = JobBuilder.Create<ProcessRefreshReportsJob>()
                                 .WithIdentity("Job4", "group4")
                                    .Build();
                //Trigger the job to run              
                ITrigger GetRefreshReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger4", "group4")
                     .WithCronSchedule("0 0/5 3-19 1/1 * ? *")// Runs Every 5 minutes from 03:00 AM - 07:57 PM
                     .ForJob(GetRefreshReportsJob)
                     .Build();

                //define the job and tie it to our ProcessFailedReportsJob Class   
                IJobDetail GetFailedReportsJob = JobBuilder.Create<ProcessFailedReportsJob>()
                                 .WithIdentity("Job5", "group5")
                                    .Build();
                //Trigger the job to run              
                ITrigger GetFailedReports_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger5", "group5")
                     .WithCronSchedule("0 10/10 3-19 1/1 * ? *")// Runs Every 10 minutes from 03:10 AM - 07:50 PM
                     .ForJob(GetFailedReportsJob)
                     .Build();

                //define the job and tie it to our ProcessOptimizations Class
                IJobDetail OptimizationsJob = JobBuilder.Create<ProcessOptimizationsJob>()
                                 .WithIdentity("Job6", "group6")
                                    .Build();
                //Trigger the job to run
                ITrigger Optimizations_trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger6", "group6")
                     .WithCronSchedule("0 0/3 21-23 1/1 * ? *")// Runs every 3 minutes from 09:00 PM
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



                //define the job and tie it to ourProcess_AnalyticsUsersCount Class   
                IJobDetail Process_UserArchieveData = JobBuilder.Create<Process_AnalyticsUsersCount>()
                                 .WithIdentity("Job9", "group9")
                                    .Build();
                //Trigger the job to run              
                ITrigger Process_UserArchieveData_Trigger = TriggerBuilder.Create()
                     .WithIdentity("trigger9", "group9")
                     .WithCronSchedule("0 30 3 * * ? *")// Runs at 03:30 AM Every day
                     .ForJob(Process_UserArchieveData)
                     .Build();
                ////define the job and tie it to our Process_DeleteDataServiceJob Class   
                //IJobDetail DeleteDataServiceJob = JobBuilder.Create<Process_DeleteDataServiceJob>()
                //                 .WithIdentity("Job10", "group10")
                //                    .Build();
                ////Trigger the job to run              
                //ITrigger DeleteDataService_trigger = TriggerBuilder.Create()
                //     .WithIdentity("trigger10", "group10")
                //     .WithCronSchedule("0 24 18 * * ? *")// Runs at 12:00 AM Every day
                //     .ForJob(DeleteDataServiceJob)
                //     .Build();

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
               // Scheduler.ScheduleJob(DeleteDataServiceJob, DeleteDataService_trigger);   //not using on live right now
                //scheduler.Shutdown();
            }

            catch (Exception err)
            {
                throw new Quartz.JobExecutionException(err);
            }
        }
    }
}
