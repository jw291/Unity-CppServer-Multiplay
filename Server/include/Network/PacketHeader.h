#pragma once

#include <cstdint>
#include <cstring>

#ifdef _WIN32
#include <winsock2.h>
#else
#include <arpa/inet.h>
#endif

// msgId, size는 네트워크 바이트 순서(빅엔디안)로 전송

struct PacketHeader
{
    static constexpr size_t SIZE = 6;

    uint16_t msgId;
    uint32_t size;

    void Serialize(char* buffer) const
    {
        uint16_t netMsgId = htons(msgId);
        uint32_t netSize = htonl(size);
        std::memcpy(buffer, &netMsgId, 2);
        std::memcpy(buffer + 2, &netSize, 4);
    }

    static PacketHeader Deserialize(const char* buffer)
    {
        PacketHeader header{};
        uint16_t netMsgId;
        uint32_t netSize;
        std::memcpy(&netMsgId, buffer, 2);
        std::memcpy(&netSize, buffer + 2, 4);
        header.msgId = ntohs(netMsgId);
        header.size = ntohl(netSize);
        return header;
    }
};
