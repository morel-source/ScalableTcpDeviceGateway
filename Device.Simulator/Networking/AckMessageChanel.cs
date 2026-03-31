using System.Threading.Channels;
using Gateway.Protocol.Enums;

namespace Device.Simulator.Networking;

public record AckMessageChanel(bool Received = false, MessageType MessageType = MessageType.Unknown)
{
    public readonly Channel<AckMessageChanel> AckChannel = Channel.CreateUnbounded<AckMessageChanel>();

    public async Task<AckMessageChanel> WaitForAckAsync(CancellationToken cancellationToken = default)
    {
        var fail = new AckMessageChanel(Received: false);
        try
        {
            // Wait for a signal from the channel
            return await AckChannel.Reader.ReadAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return fail;
        }
        catch (ChannelClosedException)
        {
            return fail;
        }
    }

    public void SignalAck(MessageType messageType)
    {
        AckChannel.Writer.TryWrite(new AckMessageChanel(Received: true, MessageType: messageType));
    }
}