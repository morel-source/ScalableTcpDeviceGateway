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
                catch (IOException ioEx)
                {
                    logger.LogDebug("Writer completion failed (expected on disconnect): {message}", ioEx.Message);
                }
                catch (ObjectDisposedException)
                {
                    /* already disposed, ignore */
                }

                try
                {
                    await context.Reader.CompleteAsync();
                }
                catch (IOException ioEx)
                {
                    logger.LogDebug("Reader completion failed (expected on disconnect): {message}", ioEx.Message);
                }
                catch (ObjectDisposedException)
                {
                    /* already disposed, ignore */
                }

                // Final cleanup
                context.TcpClient.Dispose();

                logger.LogInformation("Cleanup [{barcode}] {msg}",
                    context.DeviceBarcode, context.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected cleanup error: {message}\n {stackTrace}",
                    ex.Message, ex.StackTrace);
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
        logger.LogInformation("Closing {Count} active connections...", _connections.Count);

        var keys = _connections.Keys.ToList();

        var tasks = keys.Select(RemoveAsync);

        await Task.WhenAll(tasks);

        metrics.ResetActiveConnections();
    }

    public IEnumerable<DeviceConnectionContext> GetConnections() => _connections.Values;
}