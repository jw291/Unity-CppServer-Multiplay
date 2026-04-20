#include "Network/Session.h"
#include "Network/PacketHandler.h"
#include "GameContext.h"
#include "Util/JobQueue.h"
#include <iostream>

Session::Session(asio::ip::tcp::socket socket)
    : _socket(std::move(socket))
{
}

void Session::Start()
{
    std::cout << "[Session] Client connected: "
              << _socket.remote_endpoint() << std::endl;
    DoReadHeader();
}

void Session::DoReadHeader()
{
    auto self = shared_from_this();

    asio::async_read(
        _socket,
        asio::buffer(_headerBuf, PacketHeader::SIZE),
        [this, self](asio::error_code ec, std::size_t)
        {
            if (!ec)
            {
                PacketHeader header = PacketHeader::Deserialize(_headerBuf);
                DoReadBody(header);
            }
            else
            {
                auto session = shared_from_this();
                GameJobQueue::Instance().Push([session]() {
                    GameContext::Instance().sessionManager.Leave(session);
                });
                std::cout << "[Session] Disconnected: " << ec.message() << std::endl;
            }
        });
}

void Session::DoReadBody(PacketHeader header)
{
    auto self = shared_from_this();
    _bodyBuf.resize(header.size);

    asio::async_read(
        _socket,
        asio::buffer(_bodyBuf.data(), header.size),
        [this, self, header](asio::error_code ec, std::size_t)
        {
            if (!ec)
            {
                auto session = shared_from_this();
                auto body = std::string(_bodyBuf.data(), header.size);
                auto msgId = header.msgId;
                GameJobQueue::Instance().Push([session, body, msgId]() {
                    PacketHandler::Handle(session, msgId, body.data(), body.size());
                });

                DoReadHeader();
            }
            else
            {
                std::cout << "[Session] Read body error: " << ec.message() << std::endl;
            }
        });
}

void Session::Send(uint16_t msgId, const std::string& body)
{
    std::vector<char> packet(PacketHeader::SIZE + body.size());

    PacketHeader header{};
    header.msgId = msgId;
    header.size = static_cast<uint32_t>(body.size());
    header.Serialize(packet.data());
    std::memcpy(packet.data() + PacketHeader::SIZE, body.data(), body.size());

    auto self = shared_from_this();
    asio::post(_socket.get_executor(), [this, self, packet = std::move(packet)]() mutable {
        bool writing = !_writeQueue.empty();
        _writeQueue.push_back(std::move(packet));
        if (!writing)
            DoWrite();
    });
}

void Session::DoWrite()
{
    auto self = shared_from_this();
    asio::async_write(
        _socket,
        asio::buffer(_writeQueue.front()),
        [this, self](asio::error_code ec, std::size_t)
        {
            if (ec)
            {
                std::cout << "[Session] Send error: " << ec.message() << std::endl;
                return;
            }
            _writeQueue.pop_front();
            if (!_writeQueue.empty())
                DoWrite();
        });
}

void Session::Close()
{
    asio::error_code ec;
    _socket.close(ec);
}
