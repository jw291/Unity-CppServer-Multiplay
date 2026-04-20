#pragma once

#include "Network/PacketId.h"

namespace SkillHandler
{
    void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers);
}
