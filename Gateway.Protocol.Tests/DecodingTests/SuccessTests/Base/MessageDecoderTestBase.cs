using System.Buffers;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

public abstract class MessageDecoderTestBase<TTest, TDecoder, TPayload>
    where TTest : ITestData<MessageDecoderTestBase<TTest, TDecoder, TPayload>.TestCase>
    where TDecoder : class, IMessageDecoder<TPayload>
    where TPayload : IPayload
{
    public record TestCase(string TestName, byte[] Input, TPayload ExpectedResult);

    public static IEnumerable<object[]> GetTheoryData() =>
        TTest.TheoryData.Select(tc => new object[] { tc });

    [Xunit.Theory]
    [MemberData(nameof(GetTheoryData))]
    public void Decoder_ShouldDecodeCorrectly(TestCase testCase)
    {
        var services = new ServiceCollection();
        services.AddSingleton<TDecoder>();
        AddDependencies(services);

        var serviceProvider = services.BuildServiceProvider();
        var decoder = serviceProvider.GetRequiredService<TDecoder>();
        var parserHelper = serviceProvider.GetRequiredService<IPacketDecoderParserHelper>();

        var sequence = new ReadOnlySequence<byte>(testCase.Input);
        var success = parserHelper.GetPayloadBytesFromPacket(ref sequence, out var body, out var messageType);
        var result = decoder.Decode(body);

        Assert.True(result.Ok, result.ErrorMessage);
        Assert.Equal(testCase.ExpectedResult, result.Payload);
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<HeaderDecoderParser>();
        services.AddSingleton<MessageTypeDecoderParser>();
        services.AddSingleton<LengthDecoderParser>();
        services.AddSingleton<FooterDecoderParser>();
        services.AddSingleton<HeaderEncoderParser>();
        services.AddSingleton<MessageTypeEncoderParser>();
        services.AddSingleton<LengthEncoderParser>();
        services.AddSingleton<FooterEncoderParser>();
        services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();
    }
}