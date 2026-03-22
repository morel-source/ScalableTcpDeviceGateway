namespace Gateway.Protocol.Enums;

public enum MessageType : byte
{
    Unknown = 0x00,

    StartByte = 0x01,
    EndByte = 0x35,

    Login = 0x11,
    Heartbeat = 0x12,

    Ack = 0x22
}
