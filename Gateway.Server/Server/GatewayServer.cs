using System.Net.Sockets;
using Gateway.Server.Configuration;
using Gateway.Server.Connections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gateway.Server.Server;

public class GatewayServer(
    ILogger<GatewayServer> logger,
    IHostApplicationLifetime hostApplicationLifetime,
    IOptions<TcpServerOptions> options,
    IDeviceConnectionAcceptor connectionAcceptor,
    DeviceConnectionManager manager
) : BackgroundService
{
    private TcpListener? _listener;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the application to start
        var appStartedTcs = new TaskCompletionSource();
        await using (hostApplicationLifetime.ApplicationStarted.Register(() => appStartedTcs.SetResult()))
        {
            await appStartedTcs.Task.WaitAsync(stoppingToken).ConfigureAwait(false);
        }

        if (stoppingToken.IsCancellationRequested) return;

        int port = options.Value.ListenerPort;
        _listener = TcpListener.Create(port);
        _listener.Start(backlog: options.Value.NumberOfWaitingClients);

        logger.LogInformation("TCP Server started on port {port}", port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(stoppingToken).ConfigureAwait(false);
                logger.LogInformation("Client connected from {RemoteEndPoint}", client.Client.RemoteEndPoint);
                _ = connectionAcceptor.AcceptClient(client, stoppingToken);
            }
        }
        catch (Exception ex) when (stoppingToken.IsCancellationRequested || ex is SocketException)
        {
            // When _listener.Stop() is called, AcceptTcpClientAsync throws a SocketException.
            logger.LogInformation("TCP Listener stopped and is no longer accepting new connections.");
        }
        catch (Exception ex)
        {
            // errors that happen while the server is supposed to be running
            logger.LogCritical(ex, "TCP Listener encountered a fatal error");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Gateway Server shutdown initiated...");

        // stopping the listener  first, this immediately stops new devices from connecting and breaks the loop in ExecuteAsync
        _listener?.Stop();

        try
        {
            logger.LogInformation("Closing all active device connections...");
            await manager.CloseConnections().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during graceful connection cleanup");
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Gateway Server stopped successfully.");
    }
}