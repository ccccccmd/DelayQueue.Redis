using System.Threading.Tasks;
using DelayQueue.Abstractions;

namespace DelayQueue.Core
{
    public class ReadyQueue<T> where T : Job
    {
        private const string queuePrefix = "delay-ready-queue:";

        public Task PushToReadyQueue(string topic, T job)
        {
            return DelayedRedisHelper.RPushAsync($"{queuePrefix}{topic}", job);
        }


        public Task<T> GetJobFromReadyQueue(string topic, int timeoutSeconds)
        {
            var data = DelayedRedisHelper.BLPop<T>(timeoutSeconds, $"{queuePrefix}{topic}");
            return Task.FromResult(data);
        }
    }
}