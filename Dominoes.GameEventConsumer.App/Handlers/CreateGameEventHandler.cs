using System.Threading;
using System.Threading.Tasks;
using Hqv.Dominoes.Shared.Events.Game;
using MediatR;

namespace Hqv.Dominoes.GameEventConsumer.App.Handlers
{
    public class CreateGameEventHandler : IRequestHandler<CreateGameEvent>
    {
        public Task<Unit> Handle(CreateGameEvent request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}