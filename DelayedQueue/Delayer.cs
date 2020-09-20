using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using DelayedQueue.Abstractions;
using DelayedQueue.Core;
using Microsoft.Extensions.Logging;

namespace DelayedQueue
{

    public class Delayer
    {

        public static ConcurrentBag<string> DelayQueues = new ConcurrentBag<string>();
    }

    public class Delayer<T> : IDelayer<T> where T : Job
    {


        private readonly ILogger<Delayer<T>> _logger;

        public Delayer(ILogger<Delayer<T>> logger)
        {
            _logger = logger;
        }

        public async Task PutDealyJob(string topic, T job)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"异常的Topic");
            }

            await new JobPool<Job>().PutJobAsync(job);
            //add to bucket
            await new Bucket().PushJobToBucketAsync(topic, job.JobId, job.Delay);
        }

        public async Task RemoveDealyJob(string topic, string jobId)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"异常的Topic");
            }

            if (await new JobPool<Job>().DelJobAsync(jobId) > 0)
            {
                await new Bucket().RemoveJobAsync(topic, jobId);
            }
        }



        async Task DeliveryToReadyQueue(string topic)
        {
            var jobPool = new JobPool<T>();

            var bucket = new Bucket();

            var readyQueue = new ReadyQueue<T>();

            while (true)
            {
                try
                {
                    var jobids = await bucket.GetExpireJobsAsync(topic);
                    if (jobids == null || jobids.Length == 0)
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    foreach (var jobid in jobids)
                    {
                        var job = await jobPool.GetJobAsync(jobid);


                        if (job != null)
                        {
                            //add to ready queue
                            await readyQueue.PushToReadyQueue(topic, job);
                        }


                        await bucket.RemoveJobAsync(topic, jobid);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"DeliveryToReadyQueue Error;Topic:{topic}");

                    await Task.Delay(10000);
                }
            }
        }


        async Task ConsumeReadyJob(Func<T, Task> callback, string topic)
        {
            var jobPool = new JobPool<T>();

            var readyQueue = new ReadyQueue<T>();

            while (true)
            {
                try
                {
                    var job = await readyQueue.GetJobFromReadyQueue(topic, 3);
                    if (job == null)
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    await callback(job);

                    await jobPool.DelJobAsync(job.JobId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"ConsumeReadyJob Error; Topic:{topic}");

                    await Task.Delay(10000);
                }
            }
        }

        public void RegisterDealyedQueue(string topic, Func<T, Task> consumerDelayedJobFunc)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException($"异常的Topic");
            }

            if (Delayer.DelayQueues.Contains(topic))
            {
                return;
            }

            Task.Run(async () => await DeliveryToReadyQueue(topic));
            Task.Run(async () => await ConsumeReadyJob(consumerDelayedJobFunc, topic));

            Delayer.DelayQueues.Add(topic);
        }




    }
}