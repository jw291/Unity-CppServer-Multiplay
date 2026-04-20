#include "Game/InventoryService.h"
#include "GameContext.h"
#include "Util/JobQueue.h"

void InventoryService::GetInventory(uint32_t accountId, std::function<void(InventoryMap)> callback)
{
    GameContext::Instance().inventoryRepo.GetInventory(accountId, callback);
}

void InventoryService::UseItem(uint32_t accountId, uint32_t slotIndex,
    std::function<void(bool, uint32_t, uint32_t)> callback)
{
    // #8 fix: single DB task for GetInventory + Decrement/Remove (no nested callbacks)
    DBJobQueue::Instance().Push([accountId, slotIndex, callback] {
        auto& invRepo = GameContext::Instance().inventoryRepo;

        auto inv = invRepo.GetInventorySync(accountId);
        auto it = inv.find(slotIndex);
        if (it == inv.end())
        {
            GameJobQueue::Instance().Push([callback, slotIndex] {
                callback(false, slotIndex, 0);
            });
            return;
        }

        uint32_t itemId = it->second.first;
        uint32_t quantity = it->second.second;
        bool ok;

        if (quantity > 1)
            ok = invRepo.DecrementItemSync(accountId, slotIndex);
        else
            ok = invRepo.RemoveItemSync(accountId, slotIndex);

        GameJobQueue::Instance().Push([ok, slotIndex, itemId, callback] {
            callback(ok, slotIndex, itemId);
        });
    });
}
