#pragma once

#include "Network/PacketId.h"

namespace ShopHandler
{
    void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers);
}
