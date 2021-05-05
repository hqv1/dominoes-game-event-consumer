using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Hqv.Dominoes.GameEventConsumer.App.Data;
using Hqv.Dominoes.GameEventConsumer.App.Setup;
using Hqv.Dominoes.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hqv.Dominoes.GameEventConsumer.App.Components
{
    public class KafkaConsumer
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IMediator _mediator;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string[] _topicNames;

        public KafkaConsumer(IOptions<KafkaConsumerOptions> kafkaConsumerOptions, ILogger<KafkaConsumer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
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
        
        /// <summary>
        /// Consume Loop based on https://docs.confluent.io/clients-confluent-kafka-dotnet/current/overview.html.
        /// </summary>
        public async Task ConsumeLoop(CancellationToken cancellationToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            
            consumer.Subscribe(_topicNames);
            while (!cancellationToken.IsCancellationRequested) 
            {
                var consumeResult = consumer.Consume(cancellationToken);
                if (consumeResult == null)
                {
                    _logger.LogInformation("Kafka Consume returned null. Should only happen if cancellation occurred");
                    continue;
                }

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["ConsumeId"] = Guid.NewGuid().ToString()
                }))
                {
                    LogReceivedAMessage(consumeResult);
                    var dominoesEvent = EventJsonConverter.Convert(consumeResult.Message.Value);
                    if (dominoesEvent == null)
                    {
                        LogIssueWithConsumerResult("Message could not be deserialize into a Dominoes event.", consumeResult);
                        continue;
                    }
                    _logger.LogDebug("Sending message to mediator.");
                    await _mediator.Send(dominoesEvent, cancellationToken);
                }
            }
            consumer.Close();
        }

        private void LogReceivedAMessage(ConsumeResult<string, string> consumeResult)
        {
            _logger.LogDebug("Received message or {topic}, {partition}, {offset}, {message}",
                consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value,
                consumeResult.Message.Value);
        }

        private void LogIssueWithConsumerResult(string message, ConsumeResult<string, string> consumeResult)
        {
            _logger.LogInformation(message + " For {topic}, {partition}, {offset}, {message}",
                consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value, consumeResult.Message.Value);
        }
    }
}