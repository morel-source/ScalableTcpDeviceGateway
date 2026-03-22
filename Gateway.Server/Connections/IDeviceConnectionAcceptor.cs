using System.Net.Sockets;

namespace Gateway.Server.Connections;

public interface IDeviceConnectionAcceptor
{
    Task AcceptClient(TcpClient client, CancellationToken cancellationToken = default);
}
