#pragma once

#include <functional>
#include <optional>
#include <unordered_map>
#include <utility>
#include <cstdint>

using InventoryMap = std::unordered_map<uint32_t, std::pair<uint32_t, uint32_t>>;

class InventoryRepository
{
public:
    void GetInventory(uint32_t accountId, std::function<void(InventoryMap)> callback);
    void AddItem(uint32_t accountId, uint32_t itemId, uint32_t quantity,
                 std::function<void(std::optional<uint32_t>)> callback);
    void RemoveItem(uint32_t accountId, uint32_t slotIndex,
                    std::function<void(bool)> callback);
    void DecrementItem(uint32_t accountId, uint32_t slotIndex,
                       std::function<void(bool)> callback);

    std::optional<uint32_t> AddItemSync(uint32_t accountId, uint32_t itemId, uint32_t quantity);
    InventoryMap GetInventorySync(uint32_t accountId);
    bool DecrementItemSync(uint32_t accountId, uint32_t slotIndex);
    bool RemoveItemSync(uint32_t accountId, uint32_t slotIndex);
};
