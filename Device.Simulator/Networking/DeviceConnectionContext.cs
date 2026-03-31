using System.IO.Pipelines;
using System.Net.Sockets;

namespace Device.Simulator.Networking;

public class DeviceConnectionContext(NetworkStream stream, string deviceBarcode) : IDisposable
{
    public readonly AckMessageChanel AckMessageChanel = new();
    public PipeReader Reader { get; } = PipeReader.Create(stream);
    public PipeWriter Writer { get; } = PipeWriter.Create(stream);
    public string DeviceBarcode { get; } = deviceBarcode;

    public void Dispose()
    {
        AckMessageChanel.AckChannel.Writer.TryComplete();
        // Closing the stream handles Pipe shutdown
        stream.Dispose();
    }
}