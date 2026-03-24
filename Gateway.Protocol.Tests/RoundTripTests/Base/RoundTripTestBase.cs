using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.RoundTripTests.Base;

public abstract class RoundTripTestBase<TEncoder, TDecoder, TPayload>
    where TEncoder : class, IMessageEncoder<TPayload>
    where TDecoder : class, IMessageDecoder<TPayload>
    where TPayload : IPayload
{
    protected abstract TPayload SamplePayload { get; }

    [Fact]
    public void Should_MaintainIntegrity_Through_EncodeAndDecode()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TEncoder>();
        services.AddSingleton<TDecoder>();
        AddDependencies(services);
        var provider = services.BuildServiceProvider();

        var encoder = provider.GetRequiredService<TEncoder>();
        var decoder = provider.GetRequiredService<TDecoder>();

        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(TPayload.FixedSize);
        try
        {
            // encode
            Span<byte> buffer = sharedBuffer;
            var position = encoder.Encode(buffer, SamplePayload);

            // decode
            var sequence = new ReadOnlySequence<byte>(sharedBuffer, 0, position);
            var reader = new SequenceReader<byte>(sequence);
            var decodedResult = decoder.Decode(ref reader);

            Assert.Equal(SamplePayload, decodedResult);
            Assert.Equal(0, reader.Remaining); // Ensure we read everythin
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
    }
}