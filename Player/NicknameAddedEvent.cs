namespace PlayerDemo.Player
{
    public class NicknameAddedEvent : IEvent<PlayerAggregate>
    {
        public string Nickname { get; set; }
        public string StreamId { get; set; }

        public void Apply(PlayerAggregate agg)
        {
            agg.NickName = Nickname;
        }
    }
}