#include "Game/ShopService.h"
#include "GameContext.h"
#include "Table/DataManager.h"
#include "Util/JobQueue.h"

void ShopService::GetShopInfo(uint32_t accountId, std::function<void(uint32_t gold)> callback)
{
    GameContext::Instance().currencyRepo.GetGold(accountId, callback);
}

void ShopService::BuyItem(uint32_t accountId, uint32_t shopItemId,
    std::function<void(bool, uint32_t, uint32_t, uint32_t)> callback)
{
    const auto* shopData = DataManager::Instance().GetShopTable().Get((int)shopItemId);
    if (!shopData)
    {
        callback(false, shopItemId, 0, 0);
        return;
    }

    const auto* itemData = DataManager::Instance().GetItemTable().Get(shopData->itemId);
    if (!itemData)
    {
        callback(false, shopItemId, 0, 0);
        return;
    }

    uint32_t price = shopData->price;
    uint32_t itemId = (uint32_t)itemData->itemId;

    DBJobQueue::Instance().Push([accountId, shopItemId, itemId, price, callback] {
        auto& ctx = GameContext::Instance();

        auto remainGold = ctx.currencyRepo.SpendGoldSync(accountId, price);
        if (!remainGold.has_value())
        {
            GameJobQueue::Instance().Push([callback, shopItemId, itemId] {
                callback(false, shopItemId, itemId, 0);
            });
            return;
        }

        uint32_t gold = *remainGold;
        auto slotIndex = ctx.inventoryRepo.AddItemSync(accountId, itemId, 1);

        GameJobQueue::Instance().Push([callback, slotIndex, shopItemId, itemId, gold] {
            callback(slotIndex.has_value(), shopItemId, itemId, gold);
        });
    });
}
