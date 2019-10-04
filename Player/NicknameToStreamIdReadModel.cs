using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace PlayerDemo.Player
{
    public class NicknameToStreamIdReadModel : INotificationHandler<NicknameAddedEvent>
    {
        private static IDictionary<string, string> Awesome1337Db = new Dictionary<string, string>();

        public Task Handle(NicknameAddedEvent ev, CancellationToken cancellationToken)
        {
            Awesome1337Db.Add(ev.Nickname, ev.StreamId);
            return  Task.CompletedTask;
        }
    }
}