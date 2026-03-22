using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Device.Simulator.Networking;

public class DeviceConnectionContext(NetworkStream stream, string deviceBarcode) : IDisposable
{
    private readonly Channel<bool> _ackChannel = Channel.CreateUnbounded<bool>();
    public PipeReader Reader { get; } = PipeReader.Create(stream);
    public PipeWriter Writer { get; } = PipeWriter.Create(stream);
    public string DeviceBarcode { get; } = deviceBarcode;

    public async Task<bool> WaitForAckAsync(CancellationToken ct)
    {
        try
        {
            // Wait for a signal from the channel
            return await _ackChannel.Reader.ReadAsync(ct);
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

    public void SignalAck()
    {
        _ackChannel.Writer.TryWrite(true);
    }

    public void Dispose()
    {
        _ackChannel.Writer.TryComplete();
        // Closing the stream handles Pipe shutdown
        stream.Dispose();
    }
}