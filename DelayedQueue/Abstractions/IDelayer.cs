using System;
using System.Threading.Tasks;

namespace DelayedQueue.Abstractions
{
    public interface IDelayer<T> where T : Job
    {
        Task PutDealyJob(string topic, T job);

        void RegisterDealyedQueue(string topic, Func<T, Task> consumerDelayedJobFunc);

        Task RemoveDealyJob(string topic, string jobId);
    }
}