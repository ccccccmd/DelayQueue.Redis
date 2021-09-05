using System.Threading.Tasks;

namespace DelayQueue.Abstractions
{
    public interface IDelayedMessageProcessor
    {
        /// <summary>
        /// 添加到Ready队列
        /// </summary>
        /// <returns></returns>
        Task DeliveryToReadyQueue();

        /// <summary>
        /// 消费Ready的作业
        /// </summary>
        /// <returns></returns>
        Task ConsumeReadyJob();
    }
}