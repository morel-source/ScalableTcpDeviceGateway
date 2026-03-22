using Assert = Xunit.Assert;

namespace Gateway.Protocol.Tests.EncodingTests;

public static class AssertHelper
{
    public static void Equal(byte[] expected, byte[] actual)
    {
        if (!expected.SequenceEqual(actual))
        {
            var expectedHex = expected.Select(b => $"0x{b:X2}");
            var actualHex = actual.Select(b => $"0x{b:X2}");

            Assert.Equal(expectedHex, actualHex);
        }
    }
}