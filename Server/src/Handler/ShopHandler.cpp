#include "Handler/ShopHandler.h"
#include "Network/Session.h"
#include "GameContext.h"
#include "Table/DataManager.h"
#include "Protocol.pb.h"
#include <iostream>

namespace ShopHandler
{

static void HandleShopInfo(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    auto accountId = session->_accountId;
    GameContext::Instance().shopService.GetShopInfo(accountId,
        [session](uint32_t gold)
        {
            RES_SHOP_INFO res;
            res.set_gold(gold);
            const auto& shopAll = DataManager::Instance().GetShopTable().GetAll();
            const auto& itemTable = DataManager::Instance().GetItemTable();

            for (const auto& [shopItemId, shopData] : shopAll)
            {
                const auto* itemData = itemTable.Get(shopData.itemId);
                if (!itemData)
                    continue;

                auto* slot = res.add_items();
                slot->set_shopitemid((uint32_t)shopData.shopItemId);
                slot->set_itemid((uint32_t)shopData.itemId);
                slot->set_price(shopData.price);
            }

            session->Send(MSG_RES_SHOP_INFO, res.SerializeAsString());
        });
}

static void HandleBuyItem(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_BUY_ITEM req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_BUY_ITEM" << std::endl;
        return;
    }

    auto accountId = session->_accountId;
    auto shopItemId = req.shopitemid();
    GameContext::Instance().shopService.BuyItem(accountId, shopItemId,
        [session](bool success, uint32_t shopItemId, uint32_t itemId, uint32_t remainGold)
        {
            RES_BUY_ITEM res;
            res.set_success(success);
            res.set_shopitemid(shopItemId);
            res.set_itemid(itemId);
            res.set_remaingold(remainGold);
            session->Send(MSG_RES_BUY_ITEM, res.SerializeAsString());

            if (success)
            {
                RES_CURRENCY_UPDATE cur;
                cur.set_gold(remainGold);
                session->Send(MSG_RES_CURRENCY_UPDATE, cur.SerializeAsString());
            }
        });
}

static void HandleInventoryInfo(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    auto accountId = session->_accountId;
    GameContext::Instance().inventoryService.GetInventory(accountId,
        [session](InventoryMap inv)
        {
            RES_INVENTORY_INFO res;
            for (const auto& [slotIndex, pair] : inv)
            {
                auto* slot = res.add_slots();
                slot->set_slotindex(slotIndex);
                slot->set_itemid(pair.first);
                slot->set_quantity(pair.second);
            }

            session->Send(MSG_RES_INVENTORY_INFO, res.SerializeAsString());
        });
}

static void HandleUseItem(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_USE_ITEM req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_USE_ITEM" << std::endl;
        return;
    }

    auto accountId = session->_accountId;
    auto slotIndex = req.slotindex();
    GameContext::Instance().inventoryService.UseItem(accountId, slotIndex,
        [session, accountId](bool success, uint32_t slotIndex, uint32_t itemId)
        {
            RES_USE_ITEM res;
            res.set_success(success);
            res.set_slotindex(slotIndex);
            res.set_itemid(itemId);
            session->Send(MSG_RES_USE_ITEM, res.SerializeAsString());

            if (!success)
                return;

            const auto* itemData = DataManager::Instance().GetItemTable().Get((int)itemId);
            if (!itemData)
                return;

            if (itemData->effectType == "heal_hp")
                GameContext::Instance().playerService.HealHp(accountId, itemData->effectValue);
        });
}

void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers)
{
    handlers[MSG_REQ_SHOP_INFO] = HandleShopInfo;
    handlers[MSG_REQ_BUY_ITEM] = HandleBuyItem;
    handlers[MSG_REQ_INVENTORY_INFO] = HandleInventoryInfo;
    handlers[MSG_REQ_USE_ITEM] = HandleUseItem;
}

} // namespace ShopHandler
