using System;
using System.Threading.Tasks;

namespace PlayerDemo.Player
{
    public class CreatePlayerCommand : ICommand<PlayerAggregate>
    {
        public string StreamId { get; }
        public Guid Id { get; }

        public CreatePlayerCommand(string streamId, Guid id)
        {
            StreamId = streamId;
            Id = id;
        }
    }

    public class CreatePlayerCommandHandler : ICommandHandler<CreatePlayerCommand, PlayerAggregate>
    {
        private readonly AggregateRepository _aggreRepository;
        private readonly EventDispatcher _dispatcher;

        public CreatePlayerCommandHandler(AggregateRepository aggreRepository, EventDispatcher dispatcher)
        {
            _aggreRepository = aggreRepository;
            _dispatcher = dispatcher;
        }

        public async Task Handle(CreatePlayerCommand cmd)
        {
            var p = new PlayerAggregate() { StreamId = cmd.StreamId};
            // business rules / validation
            await _dispatcher.RaiseEvent(p, new PlayerCreatedEvent() {StreamId = cmd.StreamId, Id = cmd.Id});
        }
    }
}