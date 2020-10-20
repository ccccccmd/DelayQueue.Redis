﻿using System;
using System.Threading.Tasks;
using DelayedQueue.Abstractions;
using DelayedQueue.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DelayedQueue
{

    public interface IDelayedMessageProcessor<T> where T : Job
    {

        Task DeliveryToReadyQueue();

        Task ConsumeReadyJob();
    }

    public class DelayedMessageProcessor<T> : IDelayedMessageProcessor<T> where T : Job
    {

        private readonly ILogger<DelayedMessageProcessor<T>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DelayedMessageProcessor(ILogger<DelayedMessageProcessor<T>> logger, IServiceProvider serviceProvider)
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
                    var jobids = await bucket.GetExpireJobsAsync(topic, 10);
                    if (jobids == null || jobids.Length == 0)
                    {
                        await Task.Delay(500);
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

                    await Task.Delay(1000);
                }
            }
        }


        public async Task ConsumeReadyJob()
        {
            var mediator = _serviceProvider.GetRequiredService<IMediator>();


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