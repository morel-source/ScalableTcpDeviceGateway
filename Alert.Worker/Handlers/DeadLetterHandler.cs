using Microsoft.Extensions.Logging;

namespace Alert.Worker.Handlers;

public sealed class DeadLetterHandler(ILogger<DeadLetterHandler> logger)
{
    public void HandleDeadLetter(string deviceId, string rawHex)
    {
        logger.LogError("[DEAD-LETTER] Device={DeviceId} bad packet: {Raw}", deviceId, rawHex);
        // TODO: store for review, increment a Prometheus counter, etc.
    }
}