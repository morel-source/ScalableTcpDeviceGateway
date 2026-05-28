using System.Net.Sockets;
using Device.Simulator.Configuration;
using Device.Simulator.Messaging;
using Device.Simulator.Networking;
using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Services;

public class DeviceSimulatorService(
    ILogger<DeviceSimulatorService> logger,
    IHostApplicationLifetime hostApplicationLifetime,
    IOptions<SimulatorOptions> options,
    IMessageHandler messageHandler,
    IMetricsService metricsService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the Host to fully start before spawning devices
        var appStartedTcs = new TaskCompletionSource();
        await using (hostApplicationLifetime.ApplicationStarted.Register(() => appStartedTcs.SetResult()))
        {
            await appStartedTcs.Task.WaitAsync(stoppingToken).ConfigureAwait(false);
        }

        int deviceCount = options.Value.DeviceCount;
        metricsService.SetExpectedDevices(deviceCount);
        logger.LogInformation(message: "Starting Device Simulator: {Count} devices", deviceCount);

        // Limit CONCURRENT connection attempts to prevent server saturation
        using var connectionSemaphore = new SemaphoreSlim(options.Value.ConcurrentConnection);

        var deviceTasks = new List<Task>();

        for (int i = 1; i <= deviceCount; i++)
        {
            string deviceBarcode = i.ToString("D6");
            deviceTasks.Add(RunDeviceAsync(deviceBarcode, connectionSemaphore, stoppingToken));

            // Prevent local CPU spikes during task creation
            if (i % 100 == 0)
                await Task.Delay(5, stoppingToken);
        }

        logger.LogInformation(message: "All {Count} tasks spawned. Monitoring connections...", deviceCount);
        await Task.WhenAll(deviceTasks).ConfigureAwait(false);
    }

    private async Task RunDeviceAsync(string deviceBarcode, SemaphoreSlim loginSemaphore,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(0, options.Value.DeviceConnectionDelaySec)),
            cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            using var connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                using var tcpClient = new TcpClient();
                tcpClient.NoDelay = true;

                try
                {
                    await loginSemaphore.WaitAsync(connectionCts.Token);
                    await tcpClient.ConnectAsync(options.Value.ServerHost, options.Value.ServerPort,
                        connectionCts.Token);
                }
                finally
                {
                    loginSemaphore.Release();
                }

                using var context = new DeviceConnectionContext(tcpClient.GetStream(), deviceBarcode);

                var readerTask = RunReaderLoopAsync(context, connectionCts);

                using (metricsService.MeasureLoginProcess())
                {
                    var loginOk = await messageHandler.SendLoginAsync(context, connectionCts.Token);
                    if (!loginOk)
                    {
                        await connectionCts.CancelAsync();
                        await readerTask;
                        return;
                    }

                    metricsService.IncrementLoginConnections();
                }

                // -------------------------------------------------------
                // Run heartbeat AND telemetry concurrently.
                // WhenAny: if either loop exits (timeout, drop, cancel),
                // cancel the other one immediately and close the connection.
                // -------------------------------------------------------
                var heartbeatTask = messageHandler.SendHeartbeatLoopAsync(context, connectionCts.Token);
                var telemetryTask = messageHandler.SendTelemetryLoopAsync(context, connectionCts.Token);

                await Task.WhenAny(heartbeatTask, telemetryTask);

                await connectionCts.CancelAsync();
                await Task.WhenAll(heartbeatTask, telemetryTask, readerTask);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("[{DeviceBarcode}] connection dropped: {Message}. Retrying in 30s...",
                    deviceBarcode, ex.Message);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task RunReaderLoopAsync(DeviceConnectionContext context, CancellationTokenSource connectionCts)
    {
        try
        {
            while (!connectionCts.Token.IsCancellationRequested)
            {
                var result = await context.Reader.ReadAsync(connectionCts.Token);
                var buffer = result.Buffer;

                if (result.IsCompleted || buffer.IsEmpty)
                {
                    break; // Connection closed by server
                }

                if (messageHandler.TryParseAckFrame(ref buffer, out MessageType messageType))
                {
                    context.AckMessageChanel.SignalAck(messageType);
                }

                context.Reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or IOException)
        {
        }
        catch (Exception ex)
        {
            if (!connectionCts.Token.IsCancellationRequested)
                logger.LogError(ex, message: "Reader loop error for {DeviceBarcode}", context.DeviceBarcode);
        }
        finally
        {
            // tell the sender/heartbeat loop to stop because the reader is dead
            await connectionCts.CancelAsync();
        }
    }
}