using Device.Simulator.Configuration;
using Device.Simulator.Messaging;
using Device.Simulator.Messaging.Messages;
using Device.Simulator.Services;
using Gateway.Monitoring;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Logging;


namespace Device.Simulator.Extensions;

public static class DeviceSimulatorExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddDeviceSimulator()
        {
            builder.AddLogging(serviceName: "TcpDeviceSimulator");

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder.Services.Configure<SimulatorOptions>(
                builder.Configuration.GetSection("SimulatorOptions"));
            
            builder.Services.AddHostedService<DeviceSimulatorService>();
            builder.Services.AddSingleton<IMessageSender, TcpMessageSender>();
            builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

            builder.UseMonitoring();
            builder.AddProtocolEncoders();
            builder.AddProtocolDecoders();
            builder.AddMessageHandlers();
        }

        private void AddMessageHandlers()
        {
            builder.Services.AddSingleton<LoginMessageHandler>();
            builder.Services.AddSingleton<HeartbeatMessageHandler>();
            builder.Services.AddSingleton<AlertMessageHandler>();
            builder.Services.AddSingleton<TelemetryMessageHandler>();
            builder.Services.AddSingleton<AckMessageHandler>();
        }

        private void AddProtocolEncoders()
        {
            builder.Services.AddKeyedSingleton<IMessageEncoder, LoginMessageEncoderParser>(MessageType.Login);
            builder.Services.AddKeyedSingleton<IMessageEncoder, HeartBeatMessageEncoderParser>(MessageType.Heartbeat);
            builder.Services.AddKeyedSingleton<IMessageEncoder, TelemetryMessageEncoderParser>(MessageType.Telemetry);
            builder.Services.AddKeyedSingleton<IMessageEncoder, AlertMessageEncoderParser>(MessageType.Alert);

            builder.Services.AddSingleton<BarcodeEncoderParser>();
            builder.Services.AddSingleton<TimestampEncoderParser>();

            builder.Services.AddSingleton<TemperatureEncoderParser>();
            builder.Services.AddSingleton<BatteryEncoderParser>();
            builder.Services.AddSingleton<SignalEncoderParser>();
            builder.Services.AddSingleton<AlertTypeEncoderParser>();

            builder.Services.AddSingleton<HeaderEncoderParser>();
            builder.Services.AddSingleton<MessageTypeEncoderParser>();
            builder.Services.AddSingleton<LengthEncoderParser>();
            builder.Services.AddSingleton<FooterEncoderParser>();

            builder.Services.AddSingleton<IPacketEncoderParserHelper, PacketEncoderParserHelper>();
        }

        private void AddProtocolDecoders()
        {
            builder.Services.AddKeyedSingleton<IMessageDecoder, AckMessageDecoderParser>(MessageType.Ack);

            builder.Services.AddSingleton<HeaderDecoderParser>();
            builder.Services.AddSingleton<MessageTypeDecoderParser>();
            builder.Services.AddSingleton<LengthDecoderParser>();
            builder.Services.AddSingleton<FooterDecoderParser>();

            builder.Services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();
        }
    }
}