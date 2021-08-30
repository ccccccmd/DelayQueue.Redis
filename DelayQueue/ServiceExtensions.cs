using System;
using CSRedis;
using DelayQueue.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DelayQueue
{
    public static class ServiceExtensions
    {
        public static void AddDelayQueueService(this IServiceCollection services, IConfiguration configuration)
        {
            DelayedRedisHelper.Initialization(new CSRedisClient(configuration.GetSection("DelayQueue:Redis").Value));


            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));

            services.AddSingleton(typeof(IDelayedMessageProcessor<>), typeof(DelayedMessageProcessor<>));


            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }


        public static void AddDelayQueueService(this IServiceCollection services, string delayQueRedis)
        {
            DelayedRedisHelper.Initialization(new CSRedisClient(delayQueRedis));


            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));

            services.AddSingleton(typeof(IDelayedMessageProcessor<>), typeof(DelayedMessageProcessor<>));


            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}