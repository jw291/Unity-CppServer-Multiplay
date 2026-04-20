#include "Handler/SkillHandler.h"
#include "Network/Session.h"
#include "GameContext.h"
#include "Protocol.pb.h"
#include <iostream>

namespace SkillHandler
{

static void HandleSkillInfo(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_SKILL_INFO req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_SKILL_INFO" << std::endl;
        return;
    }

    auto accountId = session->_accountId;
    GameContext::Instance().skillService.GetSkills(accountId,
        [session, accountId](std::unordered_map<uint32_t, uint32_t> skills)
        {
            RES_SKILL_INFO res;
            res.set_accountid(accountId);
            for (auto& [skillId, skillLevel] : skills)
                (*res.mutable_skills())[skillId] = skillLevel;

            session->Send(MSG_RES_SKILL_INFO, res.SerializeAsString());
        });
}

static void HandleSkillAdd(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_SKILL_ADD req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_SKILL_ADD" << std::endl;
        return;
    }

    auto skillId = req.skillid();
    GameContext::Instance().skillService.AddSkill(session->_accountId, skillId,
        [session, skillId](std::optional<uint32_t> newLevel)
        {
            RES_SKILL_ADD res;
            res.set_success(newLevel.has_value());
            res.set_skillid(skillId);
            res.set_skilllevel(newLevel.value_or(0));
            session->Send(MSG_RES_SKILL_ADD, res.SerializeAsString());
        });
}

static void HandleSkillSubtract(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_SKILL_SUBTRACT req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_SKILL_SUBTRACT" << std::endl;
        return;
    }

    auto skillId = req.skillid();
    GameContext::Instance().skillService.SubtractSkill(session->_accountId, skillId,
        [session, skillId](std::optional<uint32_t> newLevel)
        {
            RES_SKILL_SUBTRACT res;
            res.set_success(newLevel.has_value());
            res.set_skillid(skillId);
            res.set_skilllevel(newLevel.value_or(0));
            session->Send(MSG_RES_SKILL_SUBTRACT, res.SerializeAsString());
        });
}

static void HandleSkillFire(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_SKILL_FIRE req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_SKILL_FIRE" << std::endl;
        return;
    }

    auto accountId = session->_accountId;
    auto skillId = req.skillid();
    std::vector<ProjectileInfo> projectiles;
    for (auto& projectile : req.projectiles())
        projectiles.push_back(projectile);

    GameContext::Instance().skillService.SkillFire(accountId, skillId, projectiles);
}

void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers)
{
    handlers[MSG_REQ_SKILL_INFO] = HandleSkillInfo;
    handlers[MSG_REQ_SKILL_ADD] = HandleSkillAdd;
    handlers[MSG_REQ_SKILL_SUBTRACT] = HandleSkillSubtract;
    handlers[MSG_REQ_SKILL_FIRE] = HandleSkillFire;
}

} // namespace SkillHandler
