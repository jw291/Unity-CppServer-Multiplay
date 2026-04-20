#include "Network/Server.h"

#include "../../include/Network/Session.h"
#include <iostream>
#include <memory>

Server::Server(asio::io_context& ioContext, uint16_t port)
    : _acceptor(ioContext, asio::ip::tcp::endpoint(asio::ip::tcp::v4(), port))
{
    std::cout << "[Server] Listening on port " << port << std::endl;
    DoAccept();
}

void Server::DoAccept()
{
    _acceptor.async_accept(
        [this](asio::error_code ec, asio::ip::tcp::socket socket)
        {
            if (!ec)
            {
                auto session = std::make_shared<Session>(std::move(socket));
                session->Start();
            }
            else
            {
                std::cout << "[Server] Accept error: " << ec.message() << std::endl;
            }

            DoAccept();
        });
}
