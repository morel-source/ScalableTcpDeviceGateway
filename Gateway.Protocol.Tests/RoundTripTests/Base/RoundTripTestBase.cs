using System.Buffers;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Base;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.RoundTripTests.Base;

public abstract class RoundTripTestBase<TEncoder, TDecoder, TPayload>
    where TEncoder : class, IMessageEncoder
    where TDecoder : class, IMessageDecoder<TPayload>
    where TPayload : IMessagePayload
{
    protected abstract TPayload SamplePayload { get; }
    protected abstract byte[] Input { get; }

    [Fact]
    public void EncodeDecode()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IMessageEncoder, TEncoder>(SamplePayload.MessageType);
        services.AddSingleton<TDecoder>();
        AddDependencies(services);
        var provider = services.BuildServiceProvider();

        var decoder = provider.GetRequiredService<TDecoder>();
        var decoderHelper = provider.GetRequiredService<IPacketDecoderParserHelper>();
        var encoderHelper = provider.GetRequiredService<IPacketEncoderParserHelper>();

        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(SamplePayload.FixedSize + 4);
        try
        {
            Span<byte> buffer = sharedBuffer;

            var bytesWritten = encoderHelper.EncodePayloadBytesIntoPacket(ref buffer, SamplePayload);

            var sequence = new ReadOnlySequence<byte>(sharedBuffer).Slice(0, bytesWritten);
            var success = decoderHelper.TryGetPayloadBytesFromPacket(ref sequence, out var body, out var messageType);
            var result = decoder.Decode(body);

            Assert.True(success, result.ErrorMessage);
            Assert.Equal(SamplePayload, result.Payload);
            Assert.Equal(SamplePayload.MessageType, messageType);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }
    }

    [Fact]
    public void DecodeEncode()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IMessageEncoder, TEncoder>(SamplePayload.MessageType);
        services.AddSingleton<TDecoder>();
        AddDependencies(services);
        var provider = services.BuildServiceProvider();

        var decoder = provider.GetRequiredService<TDecoder>();
        var decoderHelper = provider.GetRequiredService<IPacketDecoderParserHelper>();
        var encoderHelper = provider.GetRequiredService<IPacketEncoderParserHelper>();

        var sequence = new ReadOnlySequence<byte>(Input).Slice(0, Input.Length);
        var success = decoderHelper.TryGetPayloadBytesFromPacket(ref sequence, out var body, out var messageType);
        var result = decoder.Decode(body);
        Assert.True(success, result.ErrorMessage);

        Span<byte> buffer = Input;
        var bytesWritten = encoderHelper.EncodePayloadBytesIntoPacket(ref buffer, result.Payload);

        Assert.Equal(bytesWritten, Input.Length);
        Assert.Equal(Input, buffer);
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
        // decoders
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
        services.AddSingleton<TemperatureDecoderParser>();
        services.AddSingleton<BatteryDecoderParser>();
        services.AddSingleton<SignalDecoderParser>();
        services.AddSingleton<AlertTypeDecoderParser>();
        services.AddSingleton<HeaderDecoderParser>();
        services.AddSingleton<MessageTypeDecoderParser>();
        services.AddSingleton<LengthDecoderParser>();
        services.AddSingleton<FooterDecoderParser>();
        services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();

        // encoders
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
        services.AddSingleton<AlertTypeEncoderParser>();
        services.AddSingleton<TemperatureEncoderParser>();
        services.AddSingleton<BatteryEncoderParser>();
        services.AddSingleton<SignalEncoderParser>();
        services.AddSingleton<HeaderEncoderParser>();
        services.AddSingleton<MessageTypeEncoderParser>();
        services.AddSingleton<LengthEncoderParser>();
        services.AddSingleton<FooterEncoderParser>();
        services.AddSingleton<IPacketEncoderParserHelper, PacketEncoderParserHelper>();
    }
}