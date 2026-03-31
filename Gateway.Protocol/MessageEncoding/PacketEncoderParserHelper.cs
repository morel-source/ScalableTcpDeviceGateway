using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.MessageEncoding;

public sealed class PacketEncoderParserHelper(
    HeaderEncoderParser headerEncoderParser,
    LengthEncoderParser lengthEncoderParser,
    MessageTypeEncoderParser messageTypeEncoderParser,
    FooterEncoderParser footerEncoderParser,
    IServiceProvider serviceProvider
) : IPacketEncoderParserHelper
{
    public int EncodePayloadBytesIntoPacket<TPayload>(ref Span<byte> buffer, TPayload payload)
    {
        if (payload is not IMessagePayload payloadBytes)
        {
            throw new ArgumentException($"Payload must be of type {typeof(TPayload).Name}", nameof(payload));
        }

        var position = 0;

        headerEncoderParser.Encode(ref buffer, FrameByte.StartByte, ref position);
        messageTypeEncoderParser.Encode(ref buffer, payloadBytes.MessageType, ref position);
        lengthEncoderParser.Encode(ref buffer, payloadBytes.FixedSize, ref position);

        var encoder = serviceProvider.GetRequiredKeyedService<IMessageEncoder>(payloadBytes.MessageType);
        encoder.Encode(ref buffer, payloadBytes, ref position);

        footerEncoderParser.Encode(ref buffer, FrameByte.EndByte, ref position);

        return position;
    }
}