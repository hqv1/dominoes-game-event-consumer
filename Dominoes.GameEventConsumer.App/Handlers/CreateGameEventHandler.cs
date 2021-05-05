using System;
using System.Threading;
using System.Threading.Tasks;
using Hqv.Dominoes.GameEventConsumer.App.Data;
using Hqv.Dominoes.Shared.Events.Game;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hqv.Dominoes.GameEventConsumer.App.Handlers
{
    public class CreateGameEventHandler : IRequestHandler<CreateGameEvent>
    {
        private readonly IDbContextFactory<DominoesContext> _contextFactory;
        // private readonly DominoesContext _dominoesContext;
        //
        // public CreateGameEventHandler(DominoesContext dominoesContext)
        // {
        //     _dominoesContext = dominoesContext;
        // }

        public CreateGameEventHandler(IDbContextFactory<DominoesContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public async Task<Unit> Handle(CreateGameEvent request, CancellationToken cancellationToken)
        {
            var game = await CreateAndInsertGame(request, cancellationToken);
            
            return Unit.Value;
        }

        private async Task<Game> CreateAndInsertGame(CreateGameEvent request, CancellationToken cancellationToken)
        {
            var game = new Game
            {
                GameState = GameStateCode.Open,
                TotalPlayers =  4,
                TotalScore = 150,
                EntryCode = Guid.NewGuid().ToString(),
                CreationTimestamp = DateTime.Now,
                LastUpdateTimestamp = DateTime.Now
            };
            
            var gamePlayer = new GamePlayer
            {
                GameId = game.Id,
                EmailAddress = request.Player.Id,
                Name = request.Player.Name,
                IpAddress = request.Player.IpAddress,
                Role = GamePlayerRole.Admin,
                AccessCode = Guid.NewGuid().ToString(),
                Score = 0,
                CreationTimestamp = DateTime.Now,
                LastUpdateTimestamp = DateTime.Now
            };
            game.GamePlayers.Add(gamePlayer);

            var gameEvent = new GameEvent
            {
                GameId = game.Id,
                DominoesEvent = request,
            };
            game.GameEvents.Add(gameEvent);

            await using var dominoesContext = _contextFactory.CreateDbContext();
            dominoesContext.GameEvents.Add(gameEvent);
            await dominoesContext.SaveChangesAsync(cancellationToken);
            return game;
        }
    }
}