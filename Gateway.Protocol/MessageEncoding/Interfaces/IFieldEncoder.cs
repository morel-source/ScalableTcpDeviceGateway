namespace Gateway.Protocol.MessageEncoding.Interfaces;

public interface IFieldEncoder<in TField>
{
    void Encode(ref Span<byte> buffer, TField field, ref int position);
}