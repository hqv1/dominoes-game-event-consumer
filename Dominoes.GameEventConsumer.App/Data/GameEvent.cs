using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Hqv.Dominoes.Shared.Events;

namespace Hqv.Dominoes.GameEventConsumer.App.Data
{
    public class GameEvent
    {
        [Key] public long GameEventId { get; set; }
        public long GameId { get; set; }
        public Game? Game { get; set; }
        
        public JsonDocument? DominoesEvent { get; set; }
        //public IDominoesEvent? DominoesEvent { get; set; }
        public DateTime CreationTimestamp { get; set; }
    }
}