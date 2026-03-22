namespace Gateway.Server.Configuration;

public record TcpServerOptions(
    int ListenerPort,
    int NumberOfWaitingClients
)
{
    public TcpServerOptions() : this(
        ListenerPort: 8888,
        NumberOfWaitingClients: 1000)
    {
    }
}