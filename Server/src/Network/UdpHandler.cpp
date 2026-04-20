#include "Network/UdpHandler.h"
#include "Network/UdpServer.h"
#include "Network/PacketId.h"
#include "Protocol.pb.h"
#include <iostream>

std::unordered_map<uint16_t, UdpHandler::UdpHandlerFunc> UdpHandler::_handlers;

void UdpHandler::Init() {
    _handlers[MSG_REQ_UDP_REGISTER] = HandleUdpRegister;
    _handlers[MSG_REQ_MONSTER_MOVE] = HandleMonsterMove;
    _handlers[MSG_REQ_MOVE] = HandlePlayerMove;
}

void UdpHandler::Handle(uint16_t msgId, const char *body, uint32_t size) {
    auto it = _handlers.find(msgId);
    if (it != _handlers.end())
        it->second(body, size);
    else
        std::cout << "[UdpHandler] Unknown msgId: " << msgId << std::endl;
}

void UdpHandler::HandleUdpRegister(const char *body, uint32_t size) {
    REQ_UDP_REGISTER req;
    if (!req.ParseFromArray(body, size))
        return;

    std::cout << "[UdpHandler] UDP registered accountId: " << req.accountid() << std::endl;
}

void UdpHandler::HandlePlayerMove(const char *body, uint32_t size) {
    REQ_PLAYER_MOVE req;
    if (!req.ParseFromArray(body, size)) {
        std::cout << "[UdpHandler] Failed to parse REQ_PLAYER_MOVE" << std::endl;
        return;
    }

    RES_PLAYER_MOVE res;
    res.set_accountid(req.accountid());
    res.set_posx(req.posx());
    res.set_posy(req.posy());
    res.set_dirx(req.dirx());
    res.set_diry(req.diry());
    res.set_movespeed(req.movespeed());
    UdpServer::Instance().Broadcast(MSG_RES_MOVE, res.SerializeAsString());
}

void UdpHandler::HandleMonsterMove(const char *body, uint32_t size) {
    REQ_MONSTER_MOVE req;
    if (!req.ParseFromArray(body, size)) {
        std::cout << "[UdpHandler] Failed to parse REQ_MONSTER_MOVE" << std::endl;
        return;
    }

    RES_MONSTER_MOVE res;
    for (auto &info: req.moveinfo())
        *res.add_moveinfo() = info;

    UdpServer::Instance().Broadcast(MSG_RES_MONSTER_MOVE, res.SerializeAsString());
}
