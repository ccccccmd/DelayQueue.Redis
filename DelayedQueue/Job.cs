using System;
using System.Diagnostics.CodeAnalysis;

namespace DelayedQueue
{
    public abstract class Job
    {


        /// <summary>
        /// 保证全局唯一
        /// </summary>
        public virtual string JobId { get; set; } = Guid.NewGuid().ToString();

        public TimeSpan Delay { get; set; }

        public string Topic { get; set; }

        public string Body { get; set; }
    }
}