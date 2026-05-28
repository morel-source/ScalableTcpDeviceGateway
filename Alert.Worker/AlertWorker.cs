using Alert.Worker.Handlers;
using Confluent.Kafka;
using Kafka.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Alert.Worker;

public sealed class AlertWorker(
    ILogger<AlertWorker> logger,
    DeviceStatusHandler deviceStatusHandler,
    DeadLetterHandler deadLetterHandler
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092",
            GroupId = "alert-worker-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(topics: [Topics.DeviceStatus, Topics.TelemetryDeadLetter]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (result is null) continue;

                    switch (result.Topic)
                    {
                        case Topics.DeviceStatus:
                            deviceStatusHandler.HandleDeviceStatus(json: result.Message.Value);
                            break;
                        case Topics.TelemetryDeadLetter:
                            deadLetterHandler.HandleDeadLetter(deviceId: result.Message.Key,
                                rawHex: result.Message.Value);
                            break;
                    }

                    consumer.StoreOffset(result);
                    consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, message: "AlertWorker error");
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}