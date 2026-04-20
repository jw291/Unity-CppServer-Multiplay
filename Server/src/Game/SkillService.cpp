#include "Game/SkillService.h"
#include "GameContext.h"

void SkillService::GetSkills(uint32_t accountId,
                             std::function<void(std::unordered_map<uint32_t, uint32_t>)> callback)
{
    GameContext::Instance().skillRepo.GetSkills(accountId, callback);
}

void SkillService::AddSkill(uint32_t accountId, uint32_t skillId,
                            std::function<void(std::optional<uint32_t>)> callback)
{
    GameContext::Instance().skillRepo.AddSkill(accountId, skillId, callback);
}

void SkillService::SubtractSkill(uint32_t accountId, uint32_t skillId,
                                 std::function<void(std::optional<uint32_t>)> callback)
{
    GameContext::Instance().skillRepo.SubtractSkill(accountId, skillId, callback);
}

void SkillService::SkillFire(uint32_t accountId, uint32_t skillId,
                             const std::vector<ProjectileInfo>& projectiles)
{
    RES_SKILL_FIRE res;
    res.set_accountid(accountId);
    res.set_skillid(skillId);
    for (auto& p : projectiles)
        *res.add_projectiles() = p;

    GameContext::Instance().sessionManager.BroadCast(
        MSG_RES_SKILL_FIRE, res.SerializeAsString());
}

void SkillService::SkillStart()
{
    RES_NOTIFY_SKILL_START skillRes;
    GameContext::Instance().sessionManager.BroadCast(
        MSG_RES_SKILL_START, skillRes.SerializeAsString());
}
