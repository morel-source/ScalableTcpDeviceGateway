using System.Text.Json;
using Confluent.Kafka;
using Kafka.Contracts;
using Kafka.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telemetry.Processor.Data;

namespace Telemetry.Processor;

public sealed class TelemetryProcessorWorker(
    ILogger<TelemetryProcessorWorker> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092",
            GroupId = "telemetry-processor-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                logger.LogError("Consumer error: {Reason}", e.Reason)
            ).Build();

        consumer.Subscribe(topic: Topics.TelemetryRaw);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (result is null) continue;

                    var evt = JsonSerializer.Deserialize<TelemetryEvent>(result.Message.Value)!;

                    // create a fresh scope per message — this gives a fresh DbContext
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();

                    await ProcessAsync(evt, db, stoppingToken);

                    consumer.StoreOffset(result);
                    consumer.Commit(result);
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, message: "Bad JSON, skipping");
                    if (result is not null)
                        consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, message: "Processing failed, will retry");
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessAsync(TelemetryEvent evt, TelemetryDbContext db,
        CancellationToken cancellationToken = default)
    {
        var reading = new TelemetryReading
        {
            DeviceId = evt.DeviceId,
            Temperature = evt.Temperature,
            BatteryLevel = evt.BatteryLevel,
            SignalStrength = evt.SignalStrength,
            ReceivedAt = evt.ReceivedAt,
            StoredAt = DateTimeOffset.UtcNow,
        };

        await db.TelemetryReadings.AddAsync(reading, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Stored Device={DeviceId} Temp={Temp:F1}°C Bat={Bat}% at {Time}",
            evt.DeviceId, evt.Temperature, evt.BatteryLevel, evt.ReceivedAt);
    }
}