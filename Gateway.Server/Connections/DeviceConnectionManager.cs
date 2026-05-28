using System.Collections.Concurrent;
using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Kafka.Contracts.Events;
using Kafka.Producer;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Connections;

public sealed class DeviceConnectionManager(
    ILogger<DeviceConnectionManager> logger,
    IMetricsService metrics,
    TelemetryKafkaProducer kafkaProducer)
{
    private readonly ConcurrentDictionary<Guid, DeviceConnectionContext> _connections = new();

    public Guid Add(DeviceConnectionContext deviceConnection)
    {
        var id = Guid.NewGuid();
        if (!_connections.TryAdd(id, deviceConnection))
            return Guid.Empty;

        metrics.IncrementActiveConnections();
        return id;
    }


    public async Task RemoveAsync(Guid id)
    {
        if (_connections.TryRemove(id, out var context))
        {
            try
            {
                context.DeviceChannel.Writer.TryComplete();

                try
                {
                    await context.Writer.CompleteAsync();
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                }

                try
                {
                    await context.Reader.CompleteAsync();
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                }

                context.TcpClient.Dispose();

                logger.LogInformation("Cleanup [{DeviceBarcode}] {RemoteEndPoint}",
                    context.DeviceBarcode, context.RemoteEndPoint);

                await kafkaProducer.PublishDeviceStatusAsync(new DeviceStatusEvent
                {
                    DeviceId = context.DeviceBarcode,
                    Status = DeviceStatusType.Disconnected,
                    Timestamp = DateTimeOffset.UtcNow,
                    DisconnectReason = "connection closed"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected cleanup error");
            }
            finally
            {
                metrics.IncrementDisconnectConnections();
                metrics.DecrementActiveConnections();
            }
        }
    }

    public async Task CloseConnections()
    {
        logger.LogInformation(message: "Closing {Count} active connections...", _connections.Count);

        var keys = _connections.Keys.ToList();

        var tasks = keys.Select(RemoveAsync);

        await Task.WhenAll(tasks);

        metrics.ResetActiveConnections();
    }

    public IEnumerable<DeviceConnectionContext> GetConnections() => _connections.Values;
}