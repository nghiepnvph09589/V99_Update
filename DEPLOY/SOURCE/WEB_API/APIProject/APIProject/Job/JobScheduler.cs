using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;


namespace APIProject.Job
{
    public class JobScheduler
    {
        public async void Start()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<Jobclass>().Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("HelloJob ", "GreetingGroup")

                .WithSimpleSchedule(s => s.WithIntervalInHours(1).RepeatForever())
                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 0))
                .StartAt(DateTime.UtcNow)

                .WithPriority(1)

                .Build();

            IJobDetail jobminus = JobBuilder.Create<JobMinusPoint>().Build();

            ITrigger jobminuspoint = TriggerBuilder.Create()

                .WithIdentity("minpoint ", "minpointEverryMonth")

                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(8, 0))

                .StartAt(DateTime.UtcNow)

                .WithPriority(2)

                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.ScheduleJob(jobminus, jobminuspoint);

        }


        public async void ReStart()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Shutdown();
            Start();
        }
    }
}