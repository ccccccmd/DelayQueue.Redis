using System.Threading.Tasks;

namespace DelayQueue.Abstractions
{
    public interface IDelayer<in T> where T : Job
    {
        /// <summary>
        /// 添加延时作业
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task PutDelayJob(T job);

        /// <summary>
        /// 移除延时作业
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task RemoveDelayJob(string jobId);
    }
}