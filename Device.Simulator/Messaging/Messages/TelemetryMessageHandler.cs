using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging.Messages;

public sealed class TelemetryMessageHandler(
    ILogger<LoginMessageHandler> logger,
    IOptions<SimulatorOptions> options,
    IMessageSender messageSender,
    IPacketEncoderParserHelper packetEncoderParserHelper,
    AlertMessageHandler alertMessageHandler
)
{
    public async Task SendTelemetryLoopAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var messageType = MessageType.Telemetry;
        using var timer = new PeriodicTimer(options.Value.TelemetryInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                bool success = false;
                int retryCount = 0;

                var payload = new TelemetryMessagePayload(
                    deviceBarcode: context.DeviceBarcode,
                    temperature: context.Temperature,
                    battery: context.Battery,
                    signal: context.Strength,
                    timestamp: DateTime.Now);

                while (retryCount < 3)
                {
                    var buffer = context.Writer.GetSpan(payload.FixedSize + 4);
                    var position = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref buffer, payload);

                    success = await messageSender
                        .SendWithRetryAsync(position, context, messageType, cancellationToken)
                        .ConfigureAwait(false);

                    if (success)
                    {
                        logger.LogInformation("[{DeviceBarcode}] Telemetry sent successfully", context.DeviceBarcode);
                        break;
                    }

                    retryCount++;
                    logger.LogWarning("[{DeviceBarcode}] [{MessageType}] retry {Retry}",
                        context.DeviceBarcode, messageType.GetName(), retryCount);
                }

                if (!success)
                {
                    logger.LogWarning("[{DeviceBarcode}] [TIMEOUT] [{MessageType}] ACK missing",
                        context.DeviceBarcode, messageType.GetName());
                    break;
                }

                // ---------------------------------------------------
                // After each successful telemetry send, check thresholds.
                // If a condition is met, send an Alert packet immediately.
                // These fire independently of the telemetry send result.
                // ---------------------------------------------------
                if (context.IsHighTemperature)
                {
                    logger.LogWarning("[{DeviceBarcode}] HIGH TEMPERATURE {Temp}°C — sending alert",
                        context.DeviceBarcode, context.Temperature);
                    await alertMessageHandler.SendAlertAsync(context, AlertType.HighTemperature, cancellationToken);
                }

                if (context.IsLowBattery)
                {
                    logger.LogWarning("[{DeviceBarcode}] LOW BATTERY {Battery}% — sending alert",
                        context.DeviceBarcode, context.Battery);
                    await alertMessageHandler.SendAlertAsync(context, AlertType.LowBattery, cancellationToken);
                }

                if (context.IsWeakSignal)
                {
                    logger.LogWarning("[{DeviceBarcode}] WEAK SIGNAL {Signal} — sending alert",
                        context.DeviceBarcode, context.Strength);
                    await alertMessageHandler.SendAlertAsync(context, AlertType.SignalLost, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}