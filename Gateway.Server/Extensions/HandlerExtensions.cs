using Gateway.Protocol.Enums;
using Gateway.Server.Handlers.Base;
using Gateway.Server.Handlers.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Server.Extensions;

public static class HandlerExtensions
{
    public static void AddHandlers(this HostApplicationBuilder builder)
    {
        builder.Services.AddKeyedSingleton<IMessageHandler, LoginMessageMessageHandler>(MessageType.Login);
        builder.Services.AddKeyedSingleton<IMessageHandler, HeartbeatMessageMessageHandler>(MessageType.Heartbeat);
        builder.Services.AddKeyedSingleton<IMessageHandler, TelemetryMessageHandler>(MessageType.Telemetry);
        builder.Services.AddKeyedSingleton<IMessageHandler, AlertMessageHandler>(MessageType.Alert);
    }
}