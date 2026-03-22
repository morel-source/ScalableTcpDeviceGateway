using Gateway.Server.Connections;

namespace Gateway.Server.Messaging;

public record IncomingMessage(
    DeviceConnectionContext Context,
    byte[] Data
);