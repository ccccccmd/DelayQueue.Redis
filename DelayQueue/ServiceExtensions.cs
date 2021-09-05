using System;
using System.Linq;
using System.Threading;
using CSRedis;
using DelayQueue.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DelayQueue
{
    public static class ServiceExtensions
    {
        public static void AddDelayQueue(this IServiceCollection services, IConfiguration configuration)
        {
            AddDelayQueue(services, configuration.GetSection("DelayQueue:Redis").Value);
        }


        public static void AddDelayQueue(this IServiceCollection services, string delayQueRedis)
        {
            DelayedRedisHelper.Initialization(new CSRedisClient(delayQueRedis));


            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));

            services.AddSingleton(typeof(IDelayedMessageProcessor<>), typeof(DelayedMessageProcessor<>));


            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }
        
        public static void UseDelayQueue(this IServiceProvider serviceProvider)
        {
            var jobsType = AppDomain.CurrentDomain.GetAssemblies().Where(c => !c.IsDynamic)
                .SelectMany(c => c.GetTypes())
                .Where(c => typeof(Job).IsAssignableFrom(c) && !c.IsAbstract).ToList();

            foreach (var job in jobsType)
            {
                var processorType = typeof(IDelayedMessageProcessor<>).MakeGenericType(job);

                var processor = serviceProvider.GetService(processorType) as IDelayedMessageProcessor;

                new Thread(async () => await processor.DeliveryToReadyQueue()).Start();
                new Thread(async () => await processor.ConsumeReadyJob()).Start();
            }
        }
    }
}