#pragma once

#include <asio.hpp>
#include <memory>
#include <string>
#include <vector>
#include <deque>
#include "PacketHeader.h"
#include "SessionManager.h"

class Session : public std::enable_shared_from_this<Session>
{
public:
    Session(asio::ip::tcp::socket socket);

    void Start();
    void Send(uint16_t msgId, const std::string& body);
    void Close();
    uint32_t _accountId = 0;
    float posX = 0.f;
    float posY = 0.f;

private:
    void DoReadHeader();
    void DoReadBody(PacketHeader header);
    void DoWrite();

    asio::ip::tcp::socket _socket;
    char _headerBuf[PacketHeader::SIZE]{};
    std::vector<char> _bodyBuf;
    std::deque<std::vector<char>> _writeQueue;
};
