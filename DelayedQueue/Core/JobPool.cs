using System.Threading.Tasks;

namespace DelayedQueue.Core
{
    public class JobPool<T> where T : Job
    {

        private const string _prefix = "delay-queue-jobpool:";

        public Task<bool> PutJobAsync(T job)
        {
            //检验jobid是否全局唯一？
            return RedisHelper.SetAsync($"{_prefix}{job.JobId}", job);
        }


        public Task<T> GetJobAsync(string jobId)
        {
            return RedisHelper.GetAsync<T>($"{_prefix}{jobId}");
        }

        public Task<long> DelJobAsync(string jobId)
        {
            return RedisHelper.DelAsync($"{_prefix}{jobId}");
        }




    }
}