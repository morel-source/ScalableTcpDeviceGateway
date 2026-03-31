using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;

namespace Gateway.Protocol.MessageDecoding.Decoders.Frame;

public class MessageTypeDecoderParser : IFieldDecoder<MessageType>
{
    public Result<MessageType> Decode(ref SequenceReader<byte> buffer)
    {
        if (!buffer.TryRead(out byte msgType))
        {
            return Result<MessageType>.Failure("Missing messageType byte");
        }

        if (!Enum.IsDefined(typeof(MessageType), msgType))
            return Result<MessageType>.Failure("Invalid messageType");

        return Result<MessageType>.Success((MessageType)msgType);
    }
}