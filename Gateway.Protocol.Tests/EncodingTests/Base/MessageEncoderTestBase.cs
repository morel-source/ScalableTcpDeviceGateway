using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
        
        var result = encoder.Encode(testCase.Input);
        AssertHelper.Equal(testCase.ExpectedBuffer, result);
    }

    protected virtual void AddDependencies(IServiceCollection services){}
}