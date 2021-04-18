using System;
using System.Threading;
using Confluent.Kafka;
using Hqv.Dominoes.GameEventConsumer.App.Setup;
using Microsoft.Extensions.Options;

namespace Hqv.Dominoes.GameEventConsumer.App.Components
{
    public class KafkaConsumer
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly string[] _topicNames;

        public KafkaConsumer(IOptions<KafkaConsumerOptions> kafkaConsumerOptions)
        {
            var options = kafkaConsumerOptions.Value;
            options.Validate();

            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                GroupId = options.ClientId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _topicNames = new[] {options.TopicName?? throw new Exception("No topic name to consume. Should never occur.")};
        }
        
        public void ConsumeLoop(CancellationToken cancellationToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            
            consumer.Subscribe(_topicNames);
            while (!cancellationToken.IsCancellationRequested) 
            {
                var consumeResult = consumer.Consume(cancellationToken);
             
                // todo: what do we do with the consumed event?
            }
            consumer.Close();
        }
    }
}