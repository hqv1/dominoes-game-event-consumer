using System;
using System.ComponentModel.DataAnnotations;
using Hqv.Dominoes.Shared.Events;

namespace Hqv.Dominoes.GameEventConsumer.App.Data
{
    public class GameEvent
    {
        [Key] public long Id { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
        public IDominoesEvent? DominoesEvent { get; set; }
    }
}