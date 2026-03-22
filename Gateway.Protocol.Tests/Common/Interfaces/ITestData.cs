namespace Gateway.Protocol.Tests.Common.Interfaces;

public interface ITestData<out TTestCase>
{
    static abstract IEnumerable<TTestCase> TheoryData { get; }
}