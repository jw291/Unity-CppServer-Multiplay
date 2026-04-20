#pragma once

#include "Network/PacketId.h"

namespace CombatHandler
{
    void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers);
}
