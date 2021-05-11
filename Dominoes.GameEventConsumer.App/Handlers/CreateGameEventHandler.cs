using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Hqv.Dominoes.GameEventConsumer.App.Data;
using Hqv.Dominoes.Shared.Events.Game;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hqv.Dominoes.GameEventConsumer.App.Handlers
{
    public class CreateGameEventHandler : IRequestHandler<CreateGameEvent>
    {
        private readonly IDbContextFactory<DominoesContext> _contextFactory;
        private readonly ILogger<CreateGameEventHandler> _logger;

        public CreateGameEventHandler(IDbContextFactory<DominoesContext> contextFactory, ILogger<CreateGameEventHandler> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        
        public async Task<Unit> Handle(CreateGameEvent request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Create game");
            var game = await CreateAndInsertGame(request, cancellationToken);
            _logger.LogInformation("Game {gameId} created", game.GameId);
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
                LastUpdateTimestamp = DateTime.Now,
                GamePlayers = new List<GamePlayer>(),
                GameEvents = new List<GameEvent>()
            };
            
            var gamePlayer = new GamePlayer
            {
                GameId = game.GameId,
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
                GameId = game.GameId,
                DominoesEvent = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(request)),
            };
            game.GameEvents.Add(gameEvent);

            await using var dominoesContext = _contextFactory.CreateDbContext();
            dominoesContext.Games.Add(game);
            await dominoesContext.SaveChangesAsync(cancellationToken);
            return game;
        }
        
        
    }
}