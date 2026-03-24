using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Gateway.Server.Messaging;

namespace Gateway.Server.Connections;

public class DeviceConnectionContext
{
    public required string RemoteEndPoint { get; init; }
    public required TcpClient TcpClient { get; init; }
    public required PipeReader Reader { get; init; }
    public required PipeWriter Writer { get; init; }

    public string? DeviceBarcode
    {
        get => string.IsNullOrWhiteSpace(field) ? "Unknown" : field;
        set;
    }
    
    public Channel<IncomingMessage> DeviceChannel { get; } =
        Channel.CreateBounded<IncomingMessage>(new BoundedChannelOptions(capacity: 100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = true
        });
}