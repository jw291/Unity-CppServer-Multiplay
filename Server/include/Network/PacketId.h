#pragma once

#include <cstdint>
#include <memory>
#include <functional>
#include <unordered_map>

class Session;

enum PacketId : uint16_t
{
    MSG_REQ_LOGIN = 1,
    MSG_RES_LOGIN = 2,
    MSG_REQ_ECHO  = 3,
    MSG_RES_ECHO  = 4,
    MSG_REQ_PING  = 5,
    MSG_RES_PONG  = 6,
    MSG_REQ_MOVE  = 7,
    MSG_RES_MOVE  = 8,
    MSG_REQ_SKILL_INFO = 9,
    MSG_RES_SKILL_INFO = 10,
    MSG_REQ_SKILL_ADD  = 11,
    MSG_RES_SKILL_ADD  = 12,
    MSG_REQ_SKILL_SUBTRACT = 13,
    MSG_RES_SKILL_SUBTRACT = 14,
    MSG_REQ_SKILL_FIRE = 15,
    MSG_RES_SKILL_FIRE = 16,
    MSG_REQ_MONSTER_SPAWN = 17,
    MSG_RES_MONSTER_SPAWN = 18,
    MSG_RES_MONSTER_MOVE  = 19,
    MSG_REQ_MONSTER_HIT   = 20,
    MSG_RES_MONSTER_HIT   = 21,
    MSG_RES_MONSTER_DEAD  = 22,
    MSG_REQ_MONSTER_MOVE  = 23,
    MSG_REQ_GAME_START = 24,
    MSG_RES_GAME_START = 25,
    MSG_REQ_UDP_REGISTER  = 26,
    MSG_REQ_SHOP_INFO       = 27,
    MSG_RES_SHOP_INFO       = 28,
    MSG_REQ_BUY_ITEM        = 29,
    MSG_RES_BUY_ITEM        = 30,
    MSG_REQ_INVENTORY_INFO  = 31,
    MSG_RES_INVENTORY_INFO  = 32,
    MSG_REQ_USE_ITEM        = 33,
    MSG_RES_USE_ITEM        = 34,
    MSG_RES_MONSTER_DROP    = 35,
    MSG_RES_CURRENCY_UPDATE = 36,
    MSG_REQ_PICKUP_DROP     = 37,
    MSG_REQ_PLAYER_HIT      = 38,
    MSG_RES_PLAYER_HIT      = 39,
    MSG_RES_PICKUP_DROP     = 40,
    MSG_RES_ENTER = 1001,
    MSG_RES_LEAVE = 1002,
    MSG_RES_SKILL_START = 1003,
};

using PacketHandlerFunc = std::function<void(std::shared_ptr<Session>, const char*, uint32_t)>;
