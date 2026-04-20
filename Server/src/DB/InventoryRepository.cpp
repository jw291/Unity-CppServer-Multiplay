#include "DB/InventoryRepository.h"
#include "DB/DBManager.h"
#include "Util/JobQueue.h"
#include <iostream>

// ── Async ──────────────────────────────────────────────────

void InventoryRepository::GetInventory(uint32_t accountId, std::function<void(InventoryMap)> callback)
{
    DBJobQueue::Instance().Push([this, accountId, callback] {
        auto inv = GetInventorySync(accountId);
        GameJobQueue::Instance().Push([inv, callback] { callback(inv); });
    });
}

void InventoryRepository::AddItem(uint32_t accountId, uint32_t itemId, uint32_t quantity,
                                  std::function<void(std::optional<uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([this, accountId, itemId, quantity, callback] {
        auto result = AddItemSync(accountId, itemId, quantity);
        GameJobQueue::Instance().Push([result, callback] { callback(result); });
    });
}

void InventoryRepository::RemoveItem(uint32_t accountId, uint32_t slotIndex,
                                     std::function<void(bool)> callback)
{
    DBJobQueue::Instance().Push([this, accountId, slotIndex, callback] {
        bool ok = RemoveItemSync(accountId, slotIndex);
        GameJobQueue::Instance().Push([ok, callback] { callback(ok); });
    });
}

void InventoryRepository::DecrementItem(uint32_t accountId, uint32_t slotIndex,
                                        std::function<void(bool)> callback)
{
    DBJobQueue::Instance().Push([this, accountId, slotIndex, callback] {
        bool ok = DecrementItemSync(accountId, slotIndex);
        GameJobQueue::Instance().Push([ok, callback] { callback(ok); });
    });
}

// ── Sync (DB thread only) ──────────────────────────────────

InventoryMap InventoryRepository::GetInventorySync(uint32_t accountId)
{
    InventoryMap inv;
    try
    {
        auto& schema = DBManager::Instance().GetSchema();
        auto table = schema.getTable("inventory");
        auto result = table.select("slot_index", "item_id", "quantity")
            .where("account_id = :id")
            .bind("id", accountId)
            .execute();

        for (auto row : result)
            inv[row[0].get<uint32_t>()] = {row[1].get<uint32_t>(), row[2].get<uint32_t>()};
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[InventoryRepo] GetInventory error: " << e.what() << std::endl;
    }

    return inv;
}

std::optional<uint32_t> InventoryRepository::AddItemSync(uint32_t accountId, uint32_t itemId, uint32_t quantity)
{
    try
    {
        auto& schema = DBManager::Instance().GetSchema();
        auto table = schema.getTable("inventory");

        auto existing = table.select("slot_index", "quantity")
            .where("account_id = :a AND item_id = :i")
            .bind("a", accountId)
            .bind("i", itemId)
            .execute();

        auto row = existing.fetchOne();
        if (row)
        {
            uint32_t slotIndex = row[0].get<uint32_t>();
            uint32_t newQty = row[1].get<uint32_t>() + quantity;
            table.update()
                .set("quantity", newQty)
                .where("account_id = :a AND slot_index = :s")
                .bind("a", accountId)
                .bind("s", slotIndex)
                .execute();

            return slotIndex;
        }

        auto result = table.select("slot_index")
            .where("account_id = :id")
            .orderBy("slot_index ASC")
            .bind("id", accountId)
            .execute();

        uint32_t nextSlot = 0;
        for (auto r : result)
        {
            if (r[0].get<uint32_t>() != nextSlot)
                break;
            nextSlot++;
        }

        table.insert("account_id", "slot_index", "item_id", "quantity")
            .values(accountId, nextSlot, itemId, quantity)
            .execute();

        return nextSlot;
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[InventoryRepo] AddItem error: " << e.what() << std::endl;
        return std::nullopt;
    }
}

bool InventoryRepository::RemoveItemSync(uint32_t accountId, uint32_t slotIndex)
{
    try
    {
        auto& schema = DBManager::Instance().GetSchema();
        auto table = schema.getTable("inventory");
        auto result = table.remove()
            .where("account_id = :a AND slot_index = :s")
            .bind("a", accountId)
            .bind("s", slotIndex)
            .execute();

        return result.getAffectedItemsCount() > 0;
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[InventoryRepo] RemoveItem error: " << e.what() << std::endl;
        return false;
    }
}

bool InventoryRepository::DecrementItemSync(uint32_t accountId, uint32_t slotIndex)
{
    try
    {
        auto& schema = DBManager::Instance().GetSchema();
        auto table = schema.getTable("inventory");
        auto result = table.select("quantity")
            .where("account_id = :a AND slot_index = :s")
            .bind("a", accountId)
            .bind("s", slotIndex)
            .execute();

        auto row = result.fetchOne();
        if (!row)
            return false;

        uint32_t qty = row[0].get<uint32_t>();
        table.update()
            .set("quantity", qty - 1)
            .where("account_id = :a AND slot_index = :s")
            .bind("a", accountId)
            .bind("s", slotIndex)
            .execute();

        return true;
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[InventoryRepo] DecrementItem error: " << e.what() << std::endl;
        return false;
    }
}
