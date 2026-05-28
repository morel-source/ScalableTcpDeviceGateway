using Gateway.Monitoring;
using Gateway.Server.Configuration;
using Gateway.Server.Connections;
using Gateway.Server.Handlers;
using Gateway.Server.Messaging;
using Gateway.Server.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Logging;

namespace Gateway.Server.Extensions;

public static class GatewayServiceCollectionExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddGatewayServer()
        {
            builder.AddLogging(serviceName: "TcpDeviceGateway");

            builder.Services.Configure<TcpServerOptions>(
                builder.Configuration.GetSection("TcpServerOptions"));

            builder.Services.Configure<DeviceConnectionOptions>(
                builder.Configuration.GetSection("DeviceConnectionOptions"));

            builder.Services.AddHostedService<GatewayServer>();

            builder.Services.AddSingleton<DeviceConnectionManager>();

            builder.Services.AddSingleton<IDeviceConnectionAcceptor, TcpDeviceConnectionHandler>();

            builder.Services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

            builder.AddHandlers();
            builder.AddParsers();
            builder.AddEncoders();
            builder.UseMonitoring();
        }
    }
}