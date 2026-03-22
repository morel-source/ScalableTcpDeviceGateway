using System.Buffers;
using Microsoft.Extensions.Logging;

namespace Gateway.Protocol.Extensions;

public static class ReadOnlySequenceExtensions
{
    extension(ILogger logger)
    {
        public void LogHex(ReadOnlySequence<byte> sequence, string prefix = "")
        {
            // High-performance check: Do not execute if Debug logs are disabled
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            // A 15-byte message needs 45 chars (2 chars per byte + 1 space)
            // We use stackalloc to avoid the Heap entirely.
            int requiredChars = (int)sequence.Length * 3;
            Span<char> destination = stackalloc char[requiredChars];

            int charOffset = 0;
            foreach (var segment in sequence)
            {
                foreach (var b in segment.Span)
                {
                    // Use the built-in TryFormat to write hex directly into our span
                    b.TryFormat(destination.Slice(charOffset), out _, "X2");
                    destination[charOffset + 2] = ' '; // Add the space
                    charOffset += 3;
                }
            }

            // This still creates ONE string for the final log, 
            // but avoids the thousands of small strings StringBuilder creates.
            logger.LogDebug("{Prefix} {Buffer}", prefix, destination.ToString());
        }

        public void LogHex(byte[] bytes, string prefix = "")
        {
            // High-performance check: Do not execute if Debug logs are disabled
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            // Allocate memory on the stack: 3 chars per byte (e.g., "FF ")
            int requiredChars = bytes.Length * 3;
            Span<char> destination = stackalloc char[requiredChars];

            for (int i = 0; i < bytes.Length; i++)
            {
                // Write the byte as hex directly into the stack-allocated span
                bytes[i].TryFormat(destination.Slice(i * 3), out _, "X2");
                destination[(i * 3) + 2] = ' '; // Add the space
            }

            // Convert to string only when needed for the actual log output
            logger.LogDebug("{Prefix} {Buffer}", prefix, destination.ToString());
        }
    }
}