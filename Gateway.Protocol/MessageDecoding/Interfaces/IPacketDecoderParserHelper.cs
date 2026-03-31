using System.Buffers;
using Gateway.Protocol.Enums;

namespace Gateway.Protocol.MessageDecoding.Interfaces;

public interface IPacketDecoderParserHelper
{
    public bool GetPayloadBytesFromPacket(ref ReadOnlySequence<byte> sequence, out ReadOnlySequence<byte> body,
        out MessageType messageType);
}