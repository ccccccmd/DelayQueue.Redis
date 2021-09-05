using System.Threading.Tasks;
using DelayQueue.Abstractions;

namespace DelayQueue.Core
{
    public class ReadyQueue<T> where T : Job
    {
        private const string queuePrefix = "delay-ready-queue:";


        /// <summary>
        /// 添加到 ReadyQueue
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public Task PushToReadyQueue(string topic, T job)
        {
            return DelayedRedisHelper.RPushAsync($"{queuePrefix}{topic}", job);
        }

        /// <summary>
        /// 获取Ready的作业
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public Task<T> GetJobFromReadyQueue(string topic, int timeoutSeconds)
        {
            var data = DelayedRedisHelper.BLPop<T>(timeoutSeconds, $"{queuePrefix}{topic}");
            return Task.FromResult(data);
        }
    }
}