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
        builder.Services.AddSingleton<LoginMessageHandler>();
        builder.Services.AddSingleton<HeartbeatMessageHandler>();

        builder.Services.AddSingleton<IDictionary<MessageType, IHandler>>(sp =>
            new Dictionary<MessageType, IHandler>
            {
                { MessageType.Login, sp.GetRequiredService<LoginMessageHandler>() },
                { MessageType.Heartbeat, sp.GetRequiredService<HeartbeatMessageHandler>() }
            });
    }
}