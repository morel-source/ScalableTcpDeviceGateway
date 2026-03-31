using System.Buffers;
using Microsoft.Extensions.Logging;

namespace Gateway.Protocol.Extensions;

public static class ReadOnlySequenceExtensions
{
    public static void LogHex(this ILogger logger, ReadOnlySequence<byte> sequence, string prefix = "")
    {
        // High-performance check: Do not execute if Debug logs are disabled
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        // A 15-byte message needs 45 chars (2 chars per byte + 1 space)
        // Use stackalloc to avoid the Heap entirely.
        int requiredChars = (int)sequence.Length * 3;
        Span<char> destination = stackalloc char[requiredChars];

        int charOffset = 0;
        foreach (var segment in sequence)
        {
            foreach (var b in segment.Span)
            {
                // Use the built-in TryFormat to write hex directly into our span
                b.TryFormat(destination.Slice(charOffset), out _, format: "X2");
                destination[charOffset + 2] = ' '; // Add the space
                charOffset += 3;
            }
        }

        // This still creates ONE string for the final log, 
        // but avoids the thousands of small strings StringBuilder creates.
        logger.LogDebug(message: "{Prefix} {Buffer}", prefix, destination.ToString());
    }
}