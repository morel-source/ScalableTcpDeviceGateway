using Gateway.Protocol.MessageEncoding;
using Gateway.Protocol.MessageEncoding.Encoders.Frame;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

public abstract class MessageEncoderTestBase<TTest, TEncoder, TPayload>
    where TTest : ITestData<MessageEncoderTestBase<TTest, TEncoder, TPayload>.TestCase>
    where TEncoder : class, IMessageEncoder
    where TPayload : IPayload
{
    public record TestCase(string TestName, TPayload Input, byte[] ExpectedBuffer);

    public static IEnumerable<object[]> GetTheoryData() =>
        TTest.TheoryData.Select(tc => new object[] { tc });

    [Xunit.Theory]
    [MemberData(nameof(GetTheoryData))]
    public void Encoder_ShouldEncodeCorrectly(TestCase testCase)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMessageEncoder,TEncoder>();
        AddDependencies(services);

        var serviceProvider = services.BuildServiceProvider();
        var parserHelper = serviceProvider.GetRequiredService<IPacketEncoderParserHelper>();
        Span<byte> buffer = stackalloc byte[testCase.Input.FixedSize + 4];

        var result = parserHelper.EncodePayloadBytesIntoPacket(ref buffer, testCase.Input);
        
        Assert.Equal(testCase.ExpectedBuffer.Length, result);
        Assert.True(testCase.ExpectedBuffer.AsSpan().SequenceEqual(buffer));
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<HeaderEncoderParser>();
        services.AddSingleton<MessageTypeEncoderParser>();
        services.AddSingleton<LengthEncoderParser>();
        services.AddSingleton<FooterEncoderParser>();
        services.AddSingleton<IPacketEncoderParserHelper, PacketEncoderParserHelper>();
    }
}