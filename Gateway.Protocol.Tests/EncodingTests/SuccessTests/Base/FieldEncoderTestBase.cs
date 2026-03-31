using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

public abstract class FieldEncoderTestBase<TTest, TEncoder, TPayload>
    where TTest : ITestData<FieldEncoderTestBase<TTest, TEncoder, TPayload>.TestCase>
    where TEncoder : class, IFieldEncoder<TPayload>
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
        AddDependencies(services);

        var serviceProvider = services.BuildServiceProvider();
        var encoder = serviceProvider.GetRequiredService<TEncoder>();

        Span<byte> buffer = new byte[testCase.Input.FixedSize];
        var position = 0;

        encoder.Encode(ref buffer, testCase.Input, ref position);

        AssertHelper.Equal(testCase.ExpectedBuffer, buffer.ToArray());
        Assert.Equal(testCase.Input.FixedSize, position);
    }

    protected abstract void AddDependencies(IServiceCollection services);
}