#pragma once

#include <cstdint>
#include <functional>
#include "DB/InventoryRepository.h"

class InventoryService
{
public:
    void GetInventory(uint32_t accountId, std::function<void(InventoryMap)> callback);

    void UseItem(uint32_t accountId, uint32_t slotIndex,
                 std::function<void(bool success, uint32_t slotIndex, uint32_t itemId)> callback);
};
