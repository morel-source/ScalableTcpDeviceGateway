using System.Collections.Concurrent;
using System.Threading.Channels;
using Gateway.Protocol.Enums;

namespace Device.Simulator.Networking;

public sealed class AckMessageChanel
{
    // One channel per MessageType
    private readonly ConcurrentDictionary<MessageType, Channel<bool>> _channels = new();

    private Channel<bool> GetOrCreate(MessageType messageType) =>
        _channels.GetOrAdd(messageType, _ =>
            Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true,
            }));

    /// <summary>
    /// Called by the reader loop when an ACK arrives from the gateway.
    /// Routes the ACK to the channel for that specific MessageType.
    /// </summary>
    public void SignalAck(MessageType messageType)
    {
        var channel = GetOrCreate(messageType);
        channel.Writer.TryWrite(true);
    }

    /// <summary>
    /// Called by each sender to wait for its own ACK.
    /// Only reads from the channel for its own MessageType.
    /// </summary>
    public async Task<bool> WaitForAckAsync(MessageType messageType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = GetOrCreate(messageType);
            return await channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (ChannelClosedException)
        {
            return false;
        }
    }

    public void Complete()
    {
        foreach (var channel in _channels.Values)
            channel.Writer.TryComplete();
    }
}