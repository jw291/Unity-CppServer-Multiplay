using Google.Protobuf;

public interface ITransport
{
    void Send(PacketId msgId, IMessage message);
}