using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.TimeHelper
{
    public static class TimeHelper
    {
        private static ISchedulerFactory sf = null;
        private static IScheduler sched = null;

        static TimeHelper()
        {
            sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();
            sched.Start();
        }

        /// <summary>
        /// 添加Job 并且以定点的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="CronTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, string CronTime, string jobData) where T : IJob
        {
            IJobDetail jobCheck = JobBuilder.Create<T>().WithIdentity(JobName, JobName + "_Group").UsingJobData("jobData", jobData).Build();
            ICronTrigger CronTrigger = new CronTriggerImpl(JobName + "_CronTrigger", JobName + "_TriggerGroup", CronTime);
            return sched.ScheduleJob(jobCheck, CronTrigger);
        }

        /// <summary>
        /// 添加Job 并且以定点的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="CronTime"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, string CronTime) where T : IJob
        {
            return AddJob<T>(JobName, CronTime, null);
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, int SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, DateTime.UtcNow.AddMilliseconds(1), TimeSpan.FromMilliseconds(SimpleTime));
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, int SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, StartTime, TimeSpan.FromMilliseconds(SimpleTime));
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="SimpleTime"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, TimeSpan SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, StartTime, SimpleTime, new Dictionary<string, object>());
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="StartTime"></param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, int SimpleTime, string MapKey, object MapValue) where T : IJob
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add(MapKey, MapValue);
            return AddJob<T>(JobName, StartTime, TimeSpan.FromMilliseconds(SimpleTime), map);
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="StartTime"></param>
        /// <param name="SimpleTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, TimeSpan SimpleTime, Dictionary<string, object> map) where T : IJob
        {
            try
            {
                IJobDetail jobCheck = JobBuilder.Create<T>().WithIdentity(JobName, JobName + "_Group").Build();
                jobCheck.JobDataMap.PutAll(map);
                ISimpleTrigger triggerCheck = new SimpleTriggerImpl(JobName + "_SimpleTrigger", JobName + "_TriggerGroup",
                                            StartTime,
                                            null,
                                            SimpleTriggerImpl.RepeatIndefinitely,
                                            SimpleTime);
                return sched.ScheduleJob(jobCheck, triggerCheck);
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                throw ex;
            }


        }

        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// CronTriggerImpl类型触发器
        /// </summary>
        public static void UpdateTime(string jobName, string CronTime)
        {
            TriggerKey TKey = new TriggerKey(jobName + "_CronTrigger", jobName + "_TriggerGroup");
            CronTriggerImpl cti = sched.GetTrigger(TKey) as CronTriggerImpl;
            cti.CronExpression = new CronExpression(CronTime);
            sched.RescheduleJob(TKey, cti);
        }

        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// SimpleTriggerImpl类型触发器
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="SimpleTime">分钟数</param>
        public static void UpdateTime(string jobName, int SimpleTime)
        {
            UpdateTime(jobName, TimeSpan.FromMinutes(SimpleTime));
        }

        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// SimpleTriggerImpl类型触发器
        /// </summary>
        public static void UpdateTime(string jobName, TimeSpan SimpleTime)
        {
            TriggerKey TKey = new TriggerKey(jobName + "_SimpleTrigger", jobName + "_TriggerGroup");
            SimpleTriggerImpl sti = sched.GetTrigger(TKey) as SimpleTriggerImpl;
            sti.RepeatInterval = SimpleTime;
            sched.RescheduleJob(TKey, sti);
        }

        /// <summary>
        /// 暂停所有Job
        /// 暂停功能Quartz提供有很多,以后可扩充
        /// </summary>
        public static void PauseAll()
        {
            sched.PauseAll();
        }

        /// <summary>
        /// 恢复所有Job
        /// 恢复功能Quartz提供有很多,以后可扩充
        /// </summary>
        public static void ResumeAll()
        {
            sched.ResumeAll();
        }

        /// <summary>
        /// 删除Job
        /// 删除功能Quartz提供有很多,以后可扩充
        /// </summary>
        /// <param name="JobName"></param>
        public static void DeleteJob(string JobName)
        {
            JobKey jk = new JobKey(JobName, JobName + "_Group");
            sched.DeleteJob(jk);
        }

        /// <summary>
        /// 卸载定时器
        /// </summary>
        /// <param name="waitForJobsToComplete">是否等待job执行完成</param>
        public static void Shutdown(bool waitForJobsToComplete)
        {
            sched.Shutdown(waitForJobsToComplete);
        }

    }
}
