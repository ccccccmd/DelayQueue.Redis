## DelayQueue.Redis [![DelayQueue.Redis](https://img.shields.io/nuget/v/DelayQueue.Redis.svg)](https://www.nuget.org/packages/DelayQueue.Redis/)

> 基于Redis实现的延时队列，参考 https://tech.youzan.com/queuing_delay/



### 安装 nuget 包

> dotnet add package DelayQueue.Redis

### 定义作业

    public class TestJob : Job
    {
        public TestJob(TimeSpan delay, string body, string jobid) : base(delay, body)
        {
            JobId = jobid;
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
                "TestJobHandler消费;Topic:{Topic},JobId:{JobId},Body:{Body}," +
                $"time:{DateTime.Now:yyyy-MM-dd HH:mm:ss}", request.Topic,
                data.JobId, data.Body);

            return Task.FromResult(true);
        }
    }

### AddDelayQueue

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddDelayQueue(Configuration);
        //services.AddDealyQueue(
        //  "127.0.0.1:6379,password=ed4c3f25c68a932fef815,defaultDatabase=1");
    }

### UseDelayQueue

     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
     {
         app.ApplicationServices.UseDelayQueue();	
	 
	 }




