using System.Collections.Concurrent;
using Gateway.Monitoring.Services;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Connections;

public sealed class DeviceConnectionManager(
    ILogger<DeviceConnectionManager> logger,
    IMetricsService metrics)
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
                // Stop sending more data
                context.DeviceChannel.Writer.TryComplete();

                // Complete pipelines safely
                try
                {
                    await context.Writer.CompleteAsync();
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                    logger.LogDebug(message: "Writer completion failed (expected): {Message}", ex.Message);
                }

                try
                {
                    await context.Reader.CompleteAsync();
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                    logger.LogDebug(message: "Reader completion failed (expected on disconnect): {Message}",
                        ex.Message);
                }

                // Final cleanup
                context.TcpClient.Dispose();

                logger.LogInformation(message: "Cleanup [{DeviceBarcode}] {RemoteEndPoint}", context.DeviceBarcode,
                    context.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, message: "Unexpected cleanup error");
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