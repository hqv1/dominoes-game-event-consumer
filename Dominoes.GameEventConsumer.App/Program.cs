using System;
using System.Threading;
using Hqv.Dominoes.GameEventConsumer.App.Components;
using Hqv.Dominoes.GameEventConsumer.App.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Hqv.Dominoes.GameEventConsumer.App
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateBootstrapLogger();

            try
            {
                using var host = CreateHostBuilder(args).Build();
            
                var kafkaConsumer = host.Services.GetService<KafkaConsumer>();

                if (kafkaConsumer == null)
                {
                    throw new Exception("Unable to get a Kafka Consumer.");
                }
            
                var cancellationToken = new CancellationToken(); //todo: Use this token on shutdown
                Log.Information("Starting consumer loop");
                kafkaConsumer.ConsumeLoop(cancellationToken);
                return 0;
                //return host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => 
                    services
                    .AddOptions()
                    .Configure<KafkaConsumerOptions>(context.Configuration.GetSection(KafkaConsumerOptions.ConfigurationName))
                    .AddSingleton<KafkaConsumer>()
                    )
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(new RenderedCompactJsonFormatter())) //todo: remove and add to appsettings
            ;
    }
}