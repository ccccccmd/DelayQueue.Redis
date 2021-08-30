using System.Threading.Tasks;

namespace DelayQueue.Abstractions
{
    public interface IDelayer<in T> where T : Job
    {
        Task PutDelayJob(T job);

        Task RemoveDelayJob(string jobId);
    }
}