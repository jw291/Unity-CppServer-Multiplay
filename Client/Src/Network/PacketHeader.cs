using System;

public static class PacketHeader
{
    public const int SIZE = 6; // msgId(2) + size(4)

    public static byte[] Serialize(ushort msgId, byte[] body)
    {
        var packet = new byte[SIZE + body.Length];

        // Big-endian으로 변환
        packet[0] = (byte)(msgId >> 8);
        packet[1] = (byte)(msgId);

        var size = (uint)body.Length;
        packet[2] = (byte)(size >> 24);
        packet[3] = (byte)(size >> 16);
        packet[4] = (byte)(size >> 8);
        packet[5] = (byte)(size);

        Array.Copy(body, 0, packet, SIZE, body.Length);
        return packet;
    }

    public static (ushort msgId, uint size) Deserialize(byte[] header)
    {
        ushort msgId = (ushort)((header[0] << 8) | header[1]);
        uint size = (uint)((header[2] << 24) | (header[3] << 16) | (header[4] << 8) | header[5]);
        return (msgId, size);
    }
}
