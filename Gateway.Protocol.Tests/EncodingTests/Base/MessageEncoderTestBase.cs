using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.EncodingTests.Base;

public abstract class MessageEncoderTestBase<TTest, TEncoder, TPayload>
    where TTest : ITestData<MessageEncoderTestBase<TTest, TEncoder, TPayload>.TestCase>
    where TEncoder : class, IMessageEncoder<TPayload>
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
        services.AddSingleton<TEncoder>();
        AddDependencies(services);

        var serviceProvider = services.BuildServiceProvider();
        var encoder = serviceProvider.GetRequiredService<TEncoder>();

        Span<byte> buffer = stackalloc byte[TPayload.FixedSize];
        var result = encoder.Encode(buffer, testCase.Input);
        Assert.True(testCase.ExpectedBuffer.AsSpan().SequenceEqual(buffer[..result]));
        
        bool matches = testCase.ExpectedBuffer.AsSpan().SequenceEqual(buffer);
        Assert.True(matches, $"Test '{testCase.TestName}' failed: Buffers are not equal.");
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
    }
}