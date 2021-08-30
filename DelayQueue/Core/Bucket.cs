using System;
using System.Threading.Tasks;

namespace DelayQueue.Core
{
    public class Bucket
    {
        private const string queuePrefix = "delay-queue-bucket:";

        public Task<long> PushJobToBucketAsync(string topic, string jobId, TimeSpan delay)
        {
            var delaySec = GetDelaySeconds(delay);

            return DelayedRedisHelper.ZAddAsync($"{queuePrefix}{topic}", (delaySec, jobId));
        }


        private decimal GetDelaySeconds(TimeSpan delay)
        {
            return new DateTimeOffset(DateTime.Now.Add(delay).ToUniversalTime()).ToUnixTimeSeconds();
        }


        public Task<string[]> GetExpireJobsAsync(string topic, long limit)
        {
            return DelayedRedisHelper.ZRangeByScoreAsync<string>($"{queuePrefix}{topic}", decimal.Zero,
                GetDelaySeconds(TimeSpan.Zero), limit);
        }


        public Task<long> RemoveJobAsync(string topic, string jobId)
        {
            return DelayedRedisHelper.ZRemAsync($"{queuePrefix}{topic}", jobId);
        }


        public async Task<DateTime?> GetNextJobExecTimeAsync(string topic)
        {
            var items = await DelayedRedisHelper.ZRangeByScoreWithScoresAsync<string>($"{queuePrefix}{topic}",
                decimal.Zero,
                decimal.MaxValue
                , 1);

            if (items == null || items.Length == 0)
                return null;

            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(items[0].score.ToString("0000"))).LocalDateTime;
        }
    }
}