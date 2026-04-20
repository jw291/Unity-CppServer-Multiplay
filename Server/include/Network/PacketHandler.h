#pragma once

#include "PacketId.h"

class PacketHandler
{
public:
    static void Init();
    static void Handle(std::shared_ptr<Session> session, uint16_t msgId,
                       const char* body, uint32_t size);

private:
    static void Register(PacketId id, PacketHandlerFunc handler);
    static std::unordered_map<PacketId, PacketHandlerFunc> _handlers;
};
