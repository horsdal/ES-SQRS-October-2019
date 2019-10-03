using System;

namespace PlayerDemo.Player
{
    public class PlayerCreatedEvent : IEvent<PlayerAggregate>
    {
        public Guid Id { get; set; }
        public void Apply(PlayerAggregate agg)
        {
            agg.Id = Id;
        }
    }
}