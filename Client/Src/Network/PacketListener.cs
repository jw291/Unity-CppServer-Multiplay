using System;
using Google.Protobuf;

public static class PacketListener<T> where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> parser = new(() => new T());
    private static Action<T> listeners;

    public static void Regist(Action<T> callback)
    {
        listeners += callback;
    }

    public static void UnRegist(Action<T> callback)
    {
        listeners -= callback;
    }

    public static void Fire(byte[] body)
    {
        if (listeners == null) 
            return;
        T message = parser.ParseFrom(body);
        listeners.Invoke(message);
    }
}
