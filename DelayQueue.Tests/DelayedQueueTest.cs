using System;
using System.Threading;
using System.Threading.Tasks;
using DelayQueue.Abstractions;
using DelayQueue.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DelayQueue.Tests
{
    public class DelayQueueTest
    {

        public DelayQueueTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var service = new ServiceCollection();


            service.AddDealyedQueueService(
                "192.168.1.55:6379,password=ed4c39b015b0e46f074dbhWuEoUiZ02qWbp6d640999f25c68a932fef815,defaultDatabase=14");


            _serviceProvider = service.AddLogging(builder => { builder.AddConsole(); }).BuildServiceProvider();


            _serviceProvider.RegisterDealyedQueueJob<TestJob>();
        }

        private readonly ITestOutputHelper _testOutputHelper;




        private readonly IServiceProvider _serviceProvider;

        [Fact]
        public async Task shou_add_to_bucket()
        {
            var jobid = Guid.NewGuid().ToString("N");

            var delayer = _serviceProvider.GetRequiredService<IDelayer<TestJob>>();


            await delayer.PutDealyJob(
                new TestJob(TimeSpan.FromSeconds(10), "abcde====" + jobid, jobid));

            var jobpool = await new JobPool<TestJob>().GetJobAsync(jobid);

            Assert.NotNull(jobpool);
            Assert.True(jobpool.JobId == jobid);
            var bucketJob = await new Bucket().GetNextJobExecTimeAsync(nameof(TestJob));

            Assert.True(bucketJob != null);

            await new JobPool<TestJob>().DelJobAsync(jobid);
            await new Bucket().RemoveJobAsync(nameof(TestJob), jobid);

            DelayedRedisHelper.Del(nameof(TestJobHandler));
        }


        [Fact]
        public async Task should_consume_delay_job()
        {
            DelayedRedisHelper.Del(nameof(TestJobHandler));

            var delayer = _serviceProvider.GetRequiredService<IDelayer<TestJob>>();

            for (int j = 0; j < 100; j++)
            {
                await delayer.PutDealyJob(
                    new TestJob(TimeSpan.FromSeconds(1), "abcde====" + j, j.ToString()));
            }

            Thread.Sleep(1000 * 20);
            var num = DelayedRedisHelper.LLen(nameof(TestJobHandler));
            _testOutputHelper.WriteLine("消费：" + num);
            Assert.True(num == 100);
            _testOutputHelper.WriteLine("ok");
        }
    }







}