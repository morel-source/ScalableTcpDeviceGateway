using System.Text.Json;
using System.Text.Json.Serialization;
using Gateway.Protocol.Enums;
using Kafka.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Alert.Worker.Handlers;

public sealed class DeviceStatusHandler(ILogger<DeviceStatusHandler> logger)
{
    public void HandleDeviceStatus(string json)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        var evt = JsonSerializer.Deserialize<DeviceStatusEvent>(json, options);
        
        if (evt.Status == DeviceStatusType.Disconnected)
            logger.LogWarning("[ALERT] {DeviceId} disconnected: {Reason}", evt.DeviceId, evt.DisconnectReason);
        else
            logger.LogInformation("[ALERT] {DeviceId} connected", evt.DeviceId);
    }
}