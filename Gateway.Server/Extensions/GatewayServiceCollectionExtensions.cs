using Gateway.Monitoring;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
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
    public static void AddGatewayServer(this HostApplicationBuilder builder)
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
        builder.UseMonitoring();
    }

    extension(HostApplicationBuilder builder)
    {
        private void AddParsers()
        {
            builder.Services.AddSingleton<LoginMessageDecoderParser>();
            builder.Services.AddSingleton<HeartBeatMessageDecoderParser>();
            builder.Services.AddSingleton<IMessageEncoder<AckPayload>, AckMessageEncoderParser>();

            builder.Services.AddSingleton<BarcodeDecoderParser>();
            builder.Services.AddSingleton<TimestampDecoderParser>();


            builder.Services.AddSingleton<IDictionary<MessageType, IMessageDecoder>>(sp =>
                new Dictionary<MessageType, IMessageDecoder>
                {
                    { MessageType.Login, sp.GetRequiredService<LoginMessageDecoderParser>() },
                    { MessageType.Heartbeat, sp.GetRequiredService<HeartBeatMessageDecoderParser>() }
                });
        }

        private void AddSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.GrafanaLoki(
                    uri: "http://localhost:3100",
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