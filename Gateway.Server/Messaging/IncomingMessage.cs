using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Server.Connections;

namespace Gateway.Server.Messaging;

public record IncomingMessage(DeviceConnectionContext Context, ReadOnlySequence<byte> Data, MessageType MessageType);