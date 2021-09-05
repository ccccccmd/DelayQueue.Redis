using System;

namespace DelayQueue.Abstractions
{
    public abstract class Job
    {
        protected Job(TimeSpan delay, string body)
        {
            Delay = delay;
            Body = body;
        }

        /// <summary>
        ///     保证全局唯一
        /// </summary>
        public virtual string JobId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 延时触发时间
        /// </summary>
        public TimeSpan Delay { get; }

        public string Body { get; }
    }
}