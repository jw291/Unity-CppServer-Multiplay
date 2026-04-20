#include "../../include/Network/UdpServer.h"
#include <algorithm>
#include <iostream>
#include <cstring>
#include "../../include/Util/JobQueue.h"
#include "Network/UdpHandler.h"

void UdpServer::Init(asio::io_context& ioContext, uint16_t port)
{
    _socket = std::make_unique<asio::ip::udp::socket>(
        ioContext,
        asio::ip::udp::endpoint(asio::ip::udp::v4(), port)
    );
    std::cout << "[UdpServer] Listening on UDP port " << port << std::endl;
    DoReceive();
}

void UdpServer::RegisterEndpoint(const asio::ip::udp::endpoint& endpoint)
{
    auto it = std::find(_endpoints.begin(), _endpoints.end(), endpoint);
    if (it == _endpoints.end())
    {
        _endpoints.push_back(endpoint);
        std::cout << "[UdpServer] Registered endpoint: " << endpoint << std::endl;
    }
}

void UdpServer::Broadcast(uint16_t msgId, const std::string& body)
{
    auto packet = std::make_shared<std::vector<char>>(PacketHeader::SIZE + body.size());

    PacketHeader header{};
    header.msgId = msgId;
    header.size = static_cast<uint32_t>(body.size());
    header.Serialize(packet->data());
    std::memcpy(packet->data() + PacketHeader::SIZE, body.data(), body.size());

    auto endpoints = _endpoints;
    asio::post(_socket->get_executor(), [this, packet, endpoints]() {
        for (const auto& endpoint : endpoints)
        {
            _socket->async_send_to(
                asio::buffer(*packet),
                endpoint,
                [packet](asio::error_code ec, std::size_t)
                {
                    if (ec)
                        std::cout << "[UdpServer] Send error: " << ec.message() << std::endl;
                }
            );
        }
    });
}

void UdpServer::DoReceive()
{
    _socket->async_receive_from(
        asio::buffer(_recvBuf),
        _remoteEndpoint,
        [this](asio::error_code ec, std::size_t bytesReceived)
        {
            if (!ec && bytesReceived >= PacketHeader::SIZE)
            {
                RegisterEndpoint(_remoteEndpoint);

                auto recvBuf = _recvBuf;
                PacketHeader header = PacketHeader::Deserialize(recvBuf.data());
                uint32_t bodySize = static_cast<uint32_t>(bytesReceived) - PacketHeader::SIZE;

                GameJobQueue::Instance().Push([this, header, bodySize, recvBuf]{
                    UdpHandler::Handle(
                        header.msgId,
                        recvBuf.data() + PacketHeader::SIZE,
                        bodySize);
                });

            }
            DoReceive();
        }
    );
}