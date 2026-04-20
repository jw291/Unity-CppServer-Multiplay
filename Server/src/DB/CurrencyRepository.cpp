#include "DB/CurrencyRepository.h"
#include "DB/DBManager.h"
#include "Util/JobQueue.h"
#include <iostream>

void CurrencyRepository::GetGold(uint32_t accountId, std::function<void(uint32_t)> callback)
{
    DBJobQueue::Instance().Push([accountId, callback] {
        uint32_t gold = 0;
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("currencies");
            auto result = table.select("gold")
                .where("account_id = :id")
                .bind("id", accountId)
                .execute();

            auto row = result.fetchOne();
            if (row)
                gold = row[0].get<uint32_t>();
            else
                table.insert("account_id", "gold").values(accountId, 0).execute();
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[CurrencyRepo] GetGold error: " << e.what() << std::endl;
        }

        GameJobQueue::Instance().Push([gold, callback] { callback(gold); });
    });
}

void CurrencyRepository::AddGold(uint32_t accountId, uint32_t amount,
                                 std::function<void(uint32_t)> callback)
{
    DBJobQueue::Instance().Push([accountId, amount, callback] {
        uint32_t newGold = 0;
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("currencies");
            auto result = table.select("gold")
                .where("account_id = :id")
                .bind("id", accountId)
                .execute();

            auto row = result.fetchOne();
            if (row)
            {
                newGold = row[0].get<uint32_t>() + amount;
                table.update()
                    .set("gold", newGold)
                    .where("account_id = :id")
                    .bind("id", accountId)
                    .execute();
            }
            else
            {
                newGold = amount;
                table.insert("account_id", "gold").values(accountId, newGold).execute();
            }
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[CurrencyRepo] AddGold error: " << e.what() << std::endl;
        }

        GameJobQueue::Instance().Push([newGold, callback] { callback(newGold); });
    });
}

void CurrencyRepository::SpendGold(uint32_t accountId, uint32_t amount,
                                   std::function<void(std::optional<uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([this, accountId, amount, callback] {
        auto result = SpendGoldSync(accountId, amount);
        GameJobQueue::Instance().Push([result, callback] { callback(result); });
    });
}

std::optional<uint32_t> CurrencyRepository::SpendGoldSync(uint32_t accountId, uint32_t amount)
{
    try
    {
        auto& schema = DBManager::Instance().GetSchema();
        auto table = schema.getTable("currencies");

        // atomic: gold - amount only if gold >= amount
        auto updateResult = table.update()
            .set("gold", mysqlx::expr("gold - :cost"))
            .where("account_id = :id AND gold >= :cost")
            .bind("id", accountId)
            .bind("cost", amount)
            .execute();

        if (updateResult.getAffectedItemsCount() == 0)
            return std::nullopt;

        auto selectResult = table.select("gold")
            .where("account_id = :id")
            .bind("id", accountId)
            .execute();

        return selectResult.fetchOne()[0].get<uint32_t>();
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[CurrencyRepo] SpendGoldSync error: " << e.what() << std::endl;
        return std::nullopt;
    }
}
