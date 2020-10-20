using System;
using System.Threading;
using System.Threading.Tasks;
using DelayQueue.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DelayQueue.Redis.Sample.DelayJob
{
    public class TestJob : Job
    {



        public TestJob(TimeSpan delay, string body, string jobid) : base(delay, body)
        {
            this.JobId = jobid;
        }

        public sealed override string JobId { get; set; }
    }


    public class TestJobHandler : IRequestHandler<JobMessage<TestJob>, bool>
    {
        private readonly ILogger<TestJobHandler> _logger;

        public TestJobHandler(ILogger<TestJobHandler> logger)
        {
            _logger = logger;
        }

        public Task<bool> Handle(JobMessage<TestJob> request, CancellationToken cancellationToken)
        {
            var data = request.Data;

            _logger.LogInformation(
                $"TestJobHandler消费;Topic:{{Topic}},JobId:{{JobId}},Body:{{Body}}," +
                $"time:{DateTime.Now:yyyy-MM-dd HH:mm:ss}", request.Topic,
                data.JobId, data.Body);

            return Task.FromResult(true);
        }
    }
}