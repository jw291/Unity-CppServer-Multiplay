#include "DB/SkillRepository.h"
#include "DB/DBManager.h"
#include "Util/JobQueue.h"
#include <iostream>

void SkillRepository::GetSkills(uint32_t accountId,
                                std::function<void(std::unordered_map<uint32_t, uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([accountId, callback] {
        std::unordered_map<uint32_t, uint32_t> skills;
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("account_skills");
            auto result = table.select("skill_id", "skill_level")
                .where("account_id = :id")
                .bind("id", accountId)
                .execute();

            for (auto row : result)
                skills[row[0].get<uint32_t>()] = row[1].get<uint32_t>();
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[SkillRepo] GetSkills error: " << e.what() << std::endl;
        }

        GameJobQueue::Instance().Push([skills, callback] { callback(skills); });
    });
}

void SkillRepository::AddSkill(uint32_t accountId, uint32_t skillId,
                               std::function<void(std::optional<uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([accountId, skillId, callback] {
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("account_skills");
            auto result = table.select("skill_level")
                .where("account_id = :a AND skill_id = :s")
                .bind("a", accountId)
                .bind("s", skillId)
                .execute();

            auto row = result.fetchOne();
            if (row)
            {
                uint32_t newLevel = row[0].get<uint32_t>() + 1;
                table.update()
                    .set("skill_level", newLevel)
                    .where("account_id = :a AND skill_id = :s")
                    .bind("a", accountId)
                    .bind("s", skillId)
                    .execute();

                GameJobQueue::Instance().Push([newLevel, callback] { callback(newLevel); });
                return;
            }

            table.insert("account_id", "skill_id", "skill_level")
                .values(accountId, skillId, 1)
                .execute();

            GameJobQueue::Instance().Push([callback] { callback(1); });
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[SkillRepo] AddSkill error: " << e.what() << std::endl;
            GameJobQueue::Instance().Push([callback] { callback(std::nullopt); });
        }
    });
}

void SkillRepository::SubtractSkill(uint32_t accountId, uint32_t skillId,
                                    std::function<void(std::optional<uint32_t>)> callback)
{
    DBJobQueue::Instance().Push([accountId, skillId, callback] {
        try
        {
            auto& schema = DBManager::Instance().GetSchema();
            auto table = schema.getTable("account_skills");
            auto result = table.select("skill_level")
                .where("account_id = :a AND skill_id = :s")
                .bind("a", accountId)
                .bind("s", skillId)
                .execute();

            auto row = result.fetchOne();
            if (!row)
            {
                GameJobQueue::Instance().Push([callback] { callback(std::nullopt); });
                return;
            }

            uint32_t currentLevel = row[0].get<uint32_t>();
            if (currentLevel <= 1)
            {
                table.remove()
                    .where("account_id = :a AND skill_id = :s")
                    .bind("a", accountId)
                    .bind("s", skillId)
                    .execute();

                GameJobQueue::Instance().Push([callback] { callback(0); });
                return;
            }

            uint32_t newLevel = currentLevel - 1;
            table.update()
                .set("skill_level", newLevel)
                .where("account_id = :a AND skill_id = :s")
                .bind("a", accountId)
                .bind("s", skillId)
                .execute();

            GameJobQueue::Instance().Push([newLevel, callback] { callback(newLevel); });
        }
        catch (const mysqlx::Error& e)
        {
            std::cerr << "[SkillRepo] SubtractSkill error: " << e.what() << std::endl;
            GameJobQueue::Instance().Push([callback] { callback(std::nullopt); });
        }
    });
}
