using System;
using System.Threading;
using System.Threading.Tasks;
using CSRedis;
using DelayedQueue.Abstractions;
using DelayedQueue.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace DelayedQueue.Tests
{
    public class DelayedQueueTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        IDelayer<TestJob> _delayer = new Delayer<TestJob>(new NullLogger<Delayer<TestJob>>());



        private string _topic = "testjob";

        public DelayedQueueTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            RedisHelper.Initialization(new CSRedisClient(
                "192.168.1.55:6379,password=ed4c39b015b0e46f074dbhWuEoUiZ02qWbp6d640999f25c68a932fef815,defaultDatabase=14"));
        }

        [Fact]
        public async Task shou_add_to_bucket()
        {
            var jobid = Guid.NewGuid().ToString("N");
            await _delayer.PutDealyJob(_topic,
                new TestJob() {Body = "abcde====" + Guid.NewGuid(), Delay = TimeSpan.FromSeconds(10), JobId = jobid});

            var jobpool = await new JobPool<TestJob>().GetJobAsync(jobid);

            Assert.NotNull(jobpool);

            var bucketJob = await new Bucket().GetNextJobExecTimeAsync(_topic);

            Assert.True(bucketJob != null);

            await new JobPool<TestJob>().DelJobAsync(jobid);
            await new Bucket().RemoveJobAsync(_topic, jobid);
        }


        [Fact]
        public async Task should_consume_delay_job()
        {
            for (int j = 0; j < 100; j++)
            {
                var jobid = Guid.NewGuid().ToString("N");

                await _delayer.PutDealyJob(_topic,
                    new TestJob()
                        {Body = "abcde====" + Guid.NewGuid(), Delay = TimeSpan.FromSeconds(1), JobId = jobid});
            }


            int i = 0;

            _delayer.RegisterDealyedQueue(_topic, async job =>
            {
                _testOutputHelper.WriteLine($"消费了一个job {job.JobId}==={job.Body}", job.JobId, job.Body);
                i++;
                await Task.FromResult(1);
            });

            Thread.Sleep(1000 * 120);
            Assert.True(i == 100);
        }



    }





    public class TestJob : Job
    {

    }


}