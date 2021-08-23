using System;
using System.Threading.Tasks;
using DelayQueue.Abstractions;
using DelayQueue.Core;

namespace DelayQueue
{
    public class Delayer<T> : IDelayer<T> where T : Job
    {

        public async Task PutDelayJob(T job)
        {
            string topic = typeof(T).Name;

            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"异常的Topic");
            }

            await new JobPool<Job>().PutJobAsync(job);
            //add to bucket
            await new Bucket().PushJobToBucketAsync(topic, job.JobId, job.Delay);
        }

        public async Task RemoveDelayJob(string jobId)
        {
            string topic = typeof(T).Name;
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"异常的Topic");
            }

            if (await new JobPool<Job>().DelJobAsync(jobId) > 0)
            {
                await new Bucket().RemoveJobAsync(topic, jobId);
            }
        }






    }
}