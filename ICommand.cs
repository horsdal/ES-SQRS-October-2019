namespace PlayerDemo
{
    public interface ICommand<TAggregate> where TAggregate : AggregateBase
    {
    }
}