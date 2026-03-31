using Gateway.Monitoring;
using Gateway.Server.Configuration;
using Gateway.Server.Connections;
using Gateway.Server.Handlers;
using Gateway.Server.Messaging;
using Gateway.Server.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace Gateway.Server.Extensions;

public static class GatewayServiceCollectionExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddGatewayServer()
        {
            builder.AddSerilog();

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

        private void AddSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.GrafanaLoki(
                    uri: builder.Configuration["LokiOptions:Url"] ?? "http://localhost:3100",
                    labels:
                    [
                        new LokiLabel { Key = "Application", Value = "TcpDeviceGateway" }
                    ],
                    propertiesAsLabels: ["level"],
                    textFormatter: new Serilog.Formatting.Display.MessageTemplateTextFormatter(
                        "{Message:lj}{NewLine}{Exception}"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();
        }
    }
}