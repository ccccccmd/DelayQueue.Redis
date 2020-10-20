using System;
using CSRedis;
using DelayedQueue.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DelayedQueue
{
    public static class ServiceExtensions
    {
        public static void AddDealyedQueueService(this IServiceCollection services, IConfiguration configuration)
        {
            DelayedRedisHelper.Initialization(new CSRedisClient(configuration.GetSection("DelayedQueue:Redis").Value));


            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));

            services.AddSingleton(typeof(IDelayedMessageProcessor<>), typeof(DelayedMessageProcessor<>));


            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }


        public static void AddDealyedQueueService(this IServiceCollection services, string delayQueRedis)
        {
            DelayedRedisHelper.Initialization(new CSRedisClient(delayQueRedis));


            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));

            services.AddSingleton(typeof(IDelayedMessageProcessor<>), typeof(DelayedMessageProcessor<>));


            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }


    }
}