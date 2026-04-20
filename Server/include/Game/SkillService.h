#pragma once

#include <functional>
#include <optional>
#include <unordered_map>
#include <vector>
#include <cstdint>
#include "Protocol.pb.h"

class SkillService
{
public:
    void GetSkills(uint32_t accountId,
                   std::function<void(std::unordered_map<uint32_t, uint32_t>)> callback);

    void AddSkill(uint32_t accountId, uint32_t skillId,
                  std::function<void(std::optional<uint32_t>)> callback);

    void SubtractSkill(uint32_t accountId, uint32_t skillId,
                       std::function<void(std::optional<uint32_t>)> callback);

    void SkillFire(uint32_t accountId, uint32_t skillId,
                   const std::vector<ProjectileInfo>& projectiles);

    void SkillStart();
};
