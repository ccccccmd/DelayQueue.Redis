﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using DelayQueue.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DelayQueue
{
    public static class RegisterDelayQueueExtension
    {
        private static readonly ConcurrentBag<string> DelayQueues = new();

        public static IServiceProvider RegisterDelayQueueJob<TJob>(this IServiceProvider serviceProvider)
            where TJob : Job
        {
            var topic = typeof(TJob).Name;
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("异常的Topic");
            }

            if (DelayQueues.Contains(topic))
            {
                return serviceProvider;
            }

            var processor = serviceProvider.GetRequiredService<IDelayedMessageProcessor<TJob>>();

            new Thread(async () => await processor.DeliveryToReadyQueue()).Start();
            new Thread(async () => await processor.ConsumeReadyJob()).Start();


            DelayQueues.Add(topic);

            return serviceProvider;
        }
    }
}