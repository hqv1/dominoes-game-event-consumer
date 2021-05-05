using System;
using System.Threading;
using System.Threading.Tasks;
using Hqv.Dominoes.GameEventConsumer.App.Components;
using Hqv.Dominoes.GameEventConsumer.App.Data;
using Hqv.Dominoes.GameEventConsumer.App.Handlers;
using Hqv.Dominoes.GameEventConsumer.App.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Hqv.Dominoes.GameEventConsumer.App
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
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
                await kafkaConsumer.ConsumeLoop(cancellationToken);
                return 0;
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
                {
                    services
                        .AddOptions()
                        .Configure<KafkaConsumerOptions>(
                            context.Configuration.GetSection(KafkaConsumerOptions.ConfigurationName))
                        //.AddDbContext<DominoesContext>(options => options.UseNpgsql(context.Configuration.GetConnectionString("Dominoes")), ServiceLifetime.Singleton)
                        .AddDbContextFactory<DominoesContext>(options => options.UseNpgsql(context.Configuration.GetConnectionString("Dominoes")))
                        .AddMediatR(typeof(Program))
                        .AddSingleton<KafkaConsumer>();
                })
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services))
            ;
    }
}