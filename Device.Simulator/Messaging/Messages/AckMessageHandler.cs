using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Device.Simulator.Messaging.Messages;

public sealed class AckMessageHandler(
    ILogger<AckMessageHandler> logger,
    IServiceProvider serviceProvider,
    IPacketDecoderParserHelper packetDecoderDecoderHelper)
{
    public bool TryParseAckFrame(ref ReadOnlySequence<byte> buffer, out MessageType messageType)
    {
        messageType = MessageType.Unknown;
        try
        {
            var success = packetDecoderDecoderHelper.TryGetPayloadBytesFromPacket(
                sequence: ref buffer, out var body, out var msgType);

            if (success && msgType != MessageType.Ack)
            {
                logger.LogError("buffer is not recognize as Ack message");
                return false;
            }

            var decoder = serviceProvider.GetRequiredKeyedService<IMessageDecoder>(msgType);
            var ack = (AckMessagePayload)decoder.Decode(body);
            messageType = ack.MessageTypeAck;
            return true;
        }
        catch
        {
            return false;
        }
    }
}