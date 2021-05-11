using System;
using System.ComponentModel.DataAnnotations;

namespace Hqv.Dominoes.GameEventConsumer.App.Data
{
    public class GamePlayer
    {
        [Key] public long GamePlayerId { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string AccessCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? IpAddress { get; set; }
        public GamePlayerRole Role { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; } = null!;
        public int Score { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }
    }
}