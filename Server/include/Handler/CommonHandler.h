#pragma once

#include "Network/PacketId.h"

namespace CommonHandler
{
    void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers);
}
