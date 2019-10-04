namespace PlayerDemo.Player
{
    public class AddressAddedEvent : IEvent<PlayerAggregate>
    {
        public Address HomeAddress { get; set; }
        public string StreamId { get; set; }

        public void Apply(PlayerAggregate agg)
        {
            agg.Address = HomeAddress;
        }
    }
}