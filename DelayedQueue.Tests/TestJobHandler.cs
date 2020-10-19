using System.Threading;
using System.Threading.Tasks;
using DelayedQueue.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DelayedQueue.Tests
{

  

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

            _logger.LogInformation("TestJobHandler消费;Topic:{Topic},JobId:{JobId},Body:{Body}", request.Topic,
                data.JobId, data.Body);

            DelayedRedisHelper.RPush(nameof(TestJobHandler), data.JobId);
            return Task.FromResult(true);
        }
    }
}