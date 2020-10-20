using System;

using System.Threading.Tasks;
using DelayedQueue.Abstractions;
using DelayedQueue.Redis.Sample.DelayJob;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace DelayedQueue.Redis.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

     

        [HttpGet]
        public async Task<IActionResult> GenDelayJob([FromServices] IDelayer<TestJob> delayer)
        {
            var jobid = Guid.NewGuid().ToString();

            var delaySeconds = new Random().Next(1, 10);
            await delayer.PutDealyJob(
                new TestJob(TimeSpan.FromSeconds(delaySeconds), "abcde====" + jobid, jobid));

            return Content($"{jobid},{delaySeconds}s后执行,time:{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

    }
}