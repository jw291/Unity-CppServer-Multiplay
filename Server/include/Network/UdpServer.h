#pragma once

#include <asio.hpp>
#include <vector>
#include <string>
#include <cstdint>
#include <array>
#include "PacketHeader.h"

class UdpServer
{
public:
    static UdpServer& Instance()
    {
        static UdpServer instance;
        return instance;
    }

    void Init(asio::io_context& ioContext, uint16_t port);
    void Broadcast(uint16_t msgId, const std::string& body);
    void RegisterEndpoint(const asio::ip::udp::endpoint& endpoint);

private:
    UdpServer() = default;
    void DoReceive();

    std::unique_ptr<asio::ip::udp::socket> _socket;
    asio::ip::udp::endpoint _remoteEndpoint;
    std::array<char, 65536> _recvBuf{};
    std::vector<asio::ip::udp::endpoint> _endpoints;
};