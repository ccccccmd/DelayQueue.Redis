using MediatR;

namespace DelayQueue.Abstractions
{
    public class JobMessage<TJob> : IRequest<bool> where TJob : Job
    {
        public JobMessage(TJob data)
        {
            Data = data;
        }
        public TJob Data { get;  }
        public string Topic => typeof(TJob).Name;
    }
}