using System.Text.Json;
using Confluent.Kafka;
using Kafka.Contracts;
using Kafka.Contracts.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kafka.Producer;

public sealed class TelemetryKafkaProducer : IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<TelemetryKafkaProducer> _logger;

    public TelemetryKafkaProducer(IOptions<KafkaProducerOptions> options, ILogger<TelemetryKafkaProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = options.Value.MaxRetries,
            RetryBackoffMs = 250,
            CompressionType = CompressionType.Lz4,
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka error: {Reason} fatal={Fatal}", e.Reason, e.IsFatal)
            ).Build();
    }

    public Task PublishTelemetryAsync(TelemetryEvent evt, CancellationToken cancellationToken = default)
        => ProduceAsync(
            topic: Topics.TelemetryRaw,
            key: evt.DeviceId,
            value: evt,
            cancellationToken: cancellationToken);

    public Task PublishDeviceStatusAsync(DeviceStatusEvent evt, CancellationToken cancellationToken = default)
        => ProduceAsync(
            topic: Topics.DeviceStatus,
            key: evt.DeviceId,
            value: evt,
            cancellationToken: cancellationToken);

    public Task PublishDeadLetterAsync(string deviceId, string rawHex, CancellationToken cancellationToken = default)
        => ProduceAsync(
            topic: Topics.TelemetryDeadLetter,
            key: deviceId,
            value: rawHex,
            cancellationToken: cancellationToken);

    public Task PublishAlertAsync(DeviceAlertEvent evt, CancellationToken cancellationToken = default)
        => ProduceAsync(
            topic: Topics.TelemetryAlerts,
            key: evt.DeviceId,
            value: evt,
            cancellationToken: cancellationToken);

    private async Task ProduceAsync<T>(string topic, string key, T value, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(value),
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            _logger.LogDebug("→ {Topic} partition={P} offset={O}",
                result.Topic, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError("Kafka produce failed topic={Topic} key={Key}: {Reason}", topic, key, ex.Error.Reason);
        }
    }

    public ValueTask DisposeAsync()
    {
        _producer.Flush(timeout: TimeSpan.FromSeconds(10));
        _producer.Dispose();
        return ValueTask.CompletedTask;
    }
}