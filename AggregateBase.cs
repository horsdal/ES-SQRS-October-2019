namespace PlayerDemo
{
    public class AggregateBase
    {
        public AggregateBase()
        {
            Version = -1;
        }
        public string StreamId { get; set; }
        public int Version { get; set; }
    }
}