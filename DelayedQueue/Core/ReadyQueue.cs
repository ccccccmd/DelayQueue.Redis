using System.Threading.Tasks;

namespace DelayedQueue.Core
{
    public class ReadyQueue<T> where T : Job
    {
        private const string queuePrefix = "delay-queue-bucket:";

        public Task PushToReadyQueue(string topic, T job)
        {
            return RedisHelper.RPushAsync($"{queuePrefix}{topic}", job);
        }


        public Task<T> GetJobFromReadyQueue(string topic, int timeoutSeconds)
        {
            var data = RedisHelper.BLPop<T>(timeoutSeconds, $"{queuePrefix}{topic}");
            return Task.FromResult(data);
        }
    }
}