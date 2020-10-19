using System;
using DelayedQueue.Abstractions;

namespace DelayedQueue.Tests
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