namespace Gateway.Protocol.MessageEncoding.Interfaces;

public interface IPacketEncoderParserHelper
{
    public int EncodePayloadBytesIntoPacket<TPayload>(ref Span<byte> buffer, TPayload payload);
}