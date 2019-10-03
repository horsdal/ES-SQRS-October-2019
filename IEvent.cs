namespace PlayerDemo
{
    public interface IEvent<TAggregate>
    {
        void Apply(TAggregate agg);
    }
}