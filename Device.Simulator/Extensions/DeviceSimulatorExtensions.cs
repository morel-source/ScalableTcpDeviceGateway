using Device.Simulator.Configuration;
using Device.Simulator.Messaging;
using Device.Simulator.Services;
using Gateway.Monitoring;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace Device.Simulator.Extensions;

public static class DeviceSimulatorExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddDeviceSimulator()
        {
            builder.AddSerilog();

            builder.Services.Configure<SimulatorOptions>(
                builder.Configuration.GetSection("SimulatorOptions"));

            builder.Services.AddHostedService<DeviceSimulatorService>();

            builder.Services.AddSingleton<IMessageSender, TcpMessageSender>();
            builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

            builder.UseMonitoring();
            builder.AddProtocolEncoders();
            builder.AddProtocolDecoders();
        }

        private void AddProtocolEncoders()
        {
            builder.Services.AddSingleton<IMessageEncoder<LoginPayload>, LoginMessageEncoderParser>();
            builder.Services.AddSingleton<IMessageEncoder<HeartbeatPayload>, HeartBeatMessageEncoderParser>();
            builder.Services.AddSingleton<BarcodeEncoderParser>();
            builder.Services.AddSingleton<TimestampEncoderParser>();
        }

        private void AddProtocolDecoders()
        {
            builder.Services.AddSingleton<AckMessageDecoderParser>();
        }

        private void AddSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "TcpDeviceSimulator")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                ).WriteTo.GrafanaLoki(
                    "http://localhost:3100",
                    propertiesAsLabels: ["level", "Application"], 
                    textFormatter: new Serilog.Formatting.Display.MessageTemplateTextFormatter(
                        "{Message:lj}{NewLine}{Exception}"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();
        }
    }
}