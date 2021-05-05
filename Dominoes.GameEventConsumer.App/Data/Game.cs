using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hqv.Dominoes.GameEventConsumer.App.Data
{
    public class Game
    {
        [Key] public long Id { get; set; }
        public GameStateCode GameState { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalScore { get; set; }
        public string? EntryCode { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }

        public ICollection<GamePlayer> GamePlayers { get; set; } = null!;
        public ICollection<GameEvent> GameEvents { get; set; } = null!;
    }
}