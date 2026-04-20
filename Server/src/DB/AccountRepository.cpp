#include "DB/AccountRepository.h"
#include "DB/DBManager.h"
#include "Util/JobQueue.h"
#include <iostream>

void AccountRepository::LoginOrRegister(const std::string& username,
                                        std::function<void(std::optional<uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([username, callback] {
        std::optional<uint32_t> result;
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("accounts");
            auto res = table.select("account_id")
                .where("username = :u")
                .bind("u", username)
                .execute();

            auto row = res.fetchOne();
            if (row)
            {
                result = row[0].get<uint32_t>();
                std::cout << "[AccountRepo] Login: " << username
                          << " (accountId=" << *result << ")" << std::endl;
            }
            else
            {
                table.insert("username").values(username).execute();

                auto inserted = table.select("account_id")
                    .where("username = :u")
                    .bind("u", username)
                    .execute();
                result = inserted.fetchOne()[0].get<uint32_t>();
                std::cout << "[AccountRepo] Register: " << username
                          << " (accountId=" << *result << ")" << std::endl;
            }
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[AccountRepo] Query error: " << e.what() << std::endl;
        }

        GameJobQueue::Instance().Push([result, callback] { callback(result); });
    });
}
