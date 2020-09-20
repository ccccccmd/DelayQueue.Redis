
using DelayedQueue.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DelayedQueue
{
    public static class ServiceExtensions
    {
        public static void DealyedQueueService(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IDelayer<>), typeof(Delayer<>));
        }


       

    }
}