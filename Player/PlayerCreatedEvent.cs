using System;

namespace PlayerDemo.Player
{
    public class PlayerCreatedEvent : IEvent<PlayerAggregate>
    {
        public Guid Id { get; set; }
        public string StreamId { get; set; }

        public void Apply(PlayerAggregate agg)
        {
            agg.Id = Id;
        }
    }
}