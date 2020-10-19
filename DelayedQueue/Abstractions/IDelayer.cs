using System;
using System.Threading.Tasks;

namespace DelayedQueue.Abstractions
{
    public interface IDelayer<in T> where T : Job
    {
        Task PutDealyJob(T job);
        
        Task RemoveDealyJob(string jobId);
    }
}