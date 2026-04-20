#include "Network/PacketHandler.h"
#include "Handler/CommonHandler.h"
#include "Handler/SkillHandler.h"
#include "Handler/CombatHandler.h"
#include "Handler/ShopHandler.h"
#include <iostream>

std::unordered_map<PacketId, PacketHandlerFunc> PacketHandler::_handlers;

void PacketHandler::Init()
{
    CommonHandler::Register(_handlers);
    SkillHandler::Register(_handlers);
    CombatHandler::Register(_handlers);
    ShopHandler::Register(_handlers);
}

void PacketHandler::Register(PacketId id, PacketHandlerFunc handler)
{
    _handlers.insert({id, handler});
}

void PacketHandler::Handle(std::shared_ptr<Session> session, uint16_t msgId,
                           const char* body, uint32_t size)
{
    auto it = _handlers.find(static_cast<PacketId>(msgId));
    if (it != _handlers.end())
        it->second(session, body, size);
    else
        std::cout << "[PacketHandler] Unknown msgId: " << msgId << std::endl;
}
