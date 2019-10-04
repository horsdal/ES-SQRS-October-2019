using System.Threading.Tasks;

namespace PlayerDemo
{
    public interface ICommandHandler<TCommand, TAggregate>
        where TCommand : ICommand<TAggregate>
        where TAggregate : AggregateBase
    {
        Task Handle(TCommand command);
    }
}