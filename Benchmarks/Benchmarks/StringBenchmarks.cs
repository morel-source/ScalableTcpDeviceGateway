using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;

namespace Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringBenchmarks
{
    [Benchmark]
    public void GetStringWithToString()
    {
        var enumString = MessageType.Login.ToString();
    }

    [Benchmark]
    public void GetStringWithNameOf()
    {
        // zero allocation
        var enumString = MessageType.Login.GetName();
    }
}