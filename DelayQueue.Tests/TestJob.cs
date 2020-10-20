using System;
using DelayQueue.Abstractions;

namespace DelayQueue.Tests
{
    public class TestJob : Job
    {



        public TestJob(TimeSpan delay, string body, string jobid) : base(delay, body)
        {
            this.JobId = jobid;
        }

        public sealed override string JobId { get; set; }
    }
}