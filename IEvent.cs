using MediatR;

namespace PlayerDemo
{
    public interface IEvent<TAggregate> : INotification
    {
        string StreamId { get; set; }
        void Apply(TAggregate agg);
    }
}