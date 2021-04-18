using FluentValidation;

namespace Hqv.Dominoes.GameEventConsumer.App.Setup
{
    public class KafkaConsumerOptions
    {
        public const string ConfigurationName = "KafkaConsumer";
        
        public string? BootstrapServers { get; set; }
        public string? ClientId { get; set; }
        public string? TopicName { get; set; }
        
        public void Validate()
        {
            new KafkaProducerOptionValidator().Validate(this);
        }
        
        private class KafkaProducerOptionValidator : AbstractValidator<KafkaConsumerOptions>
        {
            public KafkaProducerOptionValidator()
            {
                RuleFor(x => x.BootstrapServers).NotEmpty();
                RuleFor(x => x.ClientId).NotEmpty();
                RuleFor(x => x.TopicName).NotEmpty();
            }
        }
    }
}