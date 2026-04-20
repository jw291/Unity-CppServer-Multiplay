#pragma once

#include <asio.hpp>
#include "SessionManager.h"

class Server
{
public:
    Server(asio::io_context& ioContext, uint16_t port);

private:
    void DoAccept();

    asio::ip::tcp::acceptor _acceptor;
};
