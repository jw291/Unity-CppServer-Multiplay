#pragma once

#include <string>
#include <vector>
#include <unordered_map>

struct SkillTableData
{
    int skillId;
    std::string skillName;
    std::string iconPath;
    int maxLevel;
};

class SkillTable
{
public:
    void Load(const std::string& path);

    const SkillTableData* Get(int skillId) const;
    const SkillTableData* Get(const std::string& skillName) const;

private:
    std::unordered_map<int, SkillTableData> _tableById;
    std::unordered_map<std::string, int> _nameToId;
};
