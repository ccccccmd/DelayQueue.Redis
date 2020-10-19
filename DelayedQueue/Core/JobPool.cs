using System.Threading.Tasks;
using DelayedQueue.Abstractions;

namespace DelayedQueue.Core
{
    public class JobPool<T> where T : Job
    {

        private const string _prefix = "delay-queue-jobpool:";

        public Task<bool> PutJobAsync(T job)
        {
            //检验jobid是否全局唯一？
            return DelayedRedisHelper.SetAsync($"{_prefix}{job.JobId}", job);
        }


        public Task<T> GetJobAsync(string jobId)
        {
            return DelayedRedisHelper.GetAsync<T>($"{_prefix}{jobId}");
        }

        public Task<long> DelJobAsync(string jobId)
        {
            return DelayedRedisHelper.DelAsync($"{_prefix}{jobId}");
        }




    }
}