using System;

namespace PlayerDemo.Player
{
    public class PlayerAggregate : AggregateBase
    {
        public string NickName { get; set; }
        public Guid Id { get; set; }

        public Address Address { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Nickname: {NickName}";
        }
    }
}