using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

public abstract class FrameDecoderTestBase<TTest, TDecoder, TPayload>
    where TTest : ITestData<FrameDecoderTestBase<TTest, TDecoder, TPayload>.TestCase>
    where TDecoder : class, IFieldDecoder<TPayload>
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

        var sequence = new ReadOnlySequence<byte>(testCase.Input);
        var reader = new SequenceReader<byte>(sequence);
        var result = decoder.Decode(ref reader);

        Assert.True(result.Ok, result.ErrorMessage);
        Assert.Equal(testCase.ExpectedResult, result.Payload);
    }

    protected virtual void AddDependencies(IServiceCollection services)
    {
    }
}