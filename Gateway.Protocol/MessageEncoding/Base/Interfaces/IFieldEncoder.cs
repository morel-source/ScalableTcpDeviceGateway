namespace Gateway.Protocol.MessageEncoding.Base.Interfaces;

public interface IFieldEncoder<in TField>
{
    void Encode(Span<byte> buffer, TField field, ref int position);
}