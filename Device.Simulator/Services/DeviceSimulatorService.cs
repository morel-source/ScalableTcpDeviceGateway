using System.Net.Sockets;
using Device.Simulator.Configuration;
using Device.Simulator.Messaging;
using Device.Simulator.Networking;
using Gateway.Monitoring.Services;
using Gateway.Protocol.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Services;

public class DeviceSimulatorService(
    ILogger<DeviceSimulatorService> logger,
    IHostApplicationLifetime hostApplicationLifetime,
    IOptions<SimulatorOptions> options,
    IMessageHandler messageHandler,
    IMetricsService metricsService) : BackgroundService
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
        logger.LogInformation("Starting Device Simulator: {deviceCount} devices", deviceCount);

        // Limit CONCURRENT connection attempts to prevent server saturation
        using var connectionSemaphore = new SemaphoreSlim(options.Value.ConcurrentConnection);

        var deviceTasks = new List<Task>();

        for (int i = 1; i <= deviceCount; i++)
        {
            string deviceAddress = i.ToString("D6");
            deviceTasks.Add(RunDeviceAsync(deviceAddress, connectionSemaphore, stoppingToken));

            // Prevent local CPU spikes during task creation
            if (i % 100 == 0)
                await Task.Delay(5, stoppingToken);
        }

        logger.LogInformation("All {count} tasks spawned. Monitoring connections...", deviceCount);
        await Task.WhenAll(deviceTasks).ConfigureAwait(false);
    }

    private async Task RunDeviceAsync(string deviceAddress, SemaphoreSlim loginSemaphore,
        CancellationToken cancellationToken = default) // Use 'ct' for app-lifetime
    {
        // Initial smear
        await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(0, options.Value.DeviceConnectionDelaySec)),
            cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            // linked to app-lifetime; allows us to kill THIS connection attempt specifically
            using var connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);


            try
            {
                using var tcpClient = new TcpClient();
                tcpClient.NoDelay = true;

                try
                {
                    // --- PHASE 1: LOGIN (Limited by Semaphore) ---
                    await loginSemaphore.WaitAsync(connectionCts.Token);
                    await tcpClient.ConnectAsync(options.Value.ServerHost, options.Value.ServerPort,
                        connectionCts.Token);
                }
                finally
                {
                    loginSemaphore.Release(); // release for the next device
                }


                using var context = new DeviceConnectionContext(tcpClient.GetStream(), deviceAddress);

                // Start background reader
                var readerTask = RunReaderLoopAsync(context, connectionCts);

                // Perform Handshake
                using (metricsService.MeasureLoginProcess())
                {
                    var loginOk = await messageHandler.SendLoginAsync(context, connectionCts.Token);
                    if (!loginOk) return; // Exit using block, triggers decrement

                    metricsService.IncrementLoginConnections();
                }

                await messageHandler.SendHeartbeatLoopAsync(context, connectionCts.Token);

                await connectionCts.CancelAsync();
                await readerTask;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("[{device}] connection dropped: {msg}. Retrying in 30s...",
                    deviceAddress, ex.Message);

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

                logger.LogHex(buffer, $"[{context.DeviceBarcode}] Rx:");

                if (result.IsCompleted || buffer.IsEmpty)
                {
                    break; // Connection closed by server
                }

                while (messageHandler.TryParseAckFrame(ref buffer))
                {
                    context.SignalAck();
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
                logger.LogError(ex, "Reader loop error for {device}", context.DeviceBarcode);
        }
        finally
        {
            // IMPORTANT: Tell the sender/heartbeat loop to stop because the reader is dead
            await connectionCts.CancelAsync();
        }
    }
}