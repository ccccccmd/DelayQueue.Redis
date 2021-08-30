using System;
using System.Threading.Tasks;
using DelayQueue.Abstractions;
using DelayQueue.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DelayQueue
{
    public interface IDelayedMessageProcessor<T> where T : Job
    {
        Task DeliveryToReadyQueue();

        Task ConsumeReadyJob();
    }

    public class DelayedMessageProcessor<T> : IDelayedMessageProcessor<T> where T : Job
    {
        private readonly ILogger<DelayedMessageProcessor<T>> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public DelayedMessageProcessor(ILogger<DelayedMessageProcessor<T>> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DeliveryToReadyQueue()
        {
            var jobPool = new JobPool<T>();


            var bucket = new Bucket();

            var readyQueue = new ReadyQueue<T>();


            var topic = typeof(T).Name;

            while (true)
            {
                try
                {
                    // todo load batchno from config options
                    var jobIds = await bucket.GetExpireJobsAsync(topic, 10);
                    if (jobIds == null || jobIds.Length == 0)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    foreach (var jobId in jobIds)
                    {
                        var job = await jobPool.GetJobAsync(jobId);


                        if (job != null)
                        {
                            //add to ready queue
                            await readyQueue.PushToReadyQueue(topic, job);
                        }


                        await bucket.RemoveJobAsync(topic, jobId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"DeliveryToReadyQueue Error;Topic:{topic}");

                    await Task.Delay(1000);
                }
            }
        }


        public async Task ConsumeReadyJob()
        {
            var jobPool = new JobPool<T>();

            var readyQueue = new ReadyQueue<T>();

            while (true)
            {
                try
                {
                    var job = await readyQueue.GetJobFromReadyQueue(typeof(T).Name, 3);
                    if (job == null)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    await jobPool.DelJobAsync(job.JobId);

                    var message = new JobMessage<T>(job);
                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"ConsumeReadyJob Error; Topic:{typeof(T).Name}");

                    await Task.Delay(1000);
                }
            }
        }
    }
}