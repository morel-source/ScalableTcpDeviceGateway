using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Server.Extensions;

public static class ParsersAndEncodersExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddParsers()
        {
            builder.Services.AddKeyedSingleton<IMessageDecoder, LoginMessageDecoderParser>(MessageType.Login);
            builder.Services.AddKeyedSingleton<IMessageDecoder, HeartBeatMessageDecoderParser>(MessageType.Heartbeat);

            builder.Services.AddSingleton<BarcodeDecoderParser>();
            builder.Services.AddSingleton<TimestampDecoderParser>();
            builder.Services.AddSingleton<HeaderDecoderParser>();
            builder.Services.AddSingleton<MessageTypeDecoderParser>();
            builder.Services.AddSingleton<LengthDecoderParser>();
            builder.Services.AddSingleton<FooterDecoderParser>();

            builder.Services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();
        }

        public void AddEncoders()
        {
            builder.Services.AddSingleton<HeaderEncoderParser>();
            builder.Services.AddSingleton<MessageTypeEncoderParser>();
            builder.Services.AddSingleton<LengthEncoderParser>();
            builder.Services.AddSingleton<FooterEncoderParser>();
            
            builder.Services.AddKeyedSingleton<IMessageEncoder, AckMessageEncoderParser>(MessageType.Ack);
            builder.Services.AddSingleton<IPacketEncoderParserHelper, PacketEncoderParserHelper>();
        }
    }
}