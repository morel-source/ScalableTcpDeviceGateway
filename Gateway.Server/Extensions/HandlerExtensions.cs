using Gateway.Protocol.Enums;
using Gateway.Server.Handlers;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Server.Extensions;

public static class HandlerExtensions
{
    public static void AddHandlers(this HostApplicationBuilder builder)
    {
        builder.Services.AddKeyedSingleton<IMessageHandler, LoginMessageMessageHandler>(MessageType.Login);
        builder.Services.AddKeyedSingleton<IMessageHandler, HeartbeatMessageMessageHandler>(MessageType.Heartbeat);
    }
}