#include "../../include/Table/SkillTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void SkillTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[SkillTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        SkillTableData data;
        data.skillId = obj["skillId"].get<int>();
        data.skillName = obj["skillName"].get<std::string>();
        data.iconPath = obj["iconPath"].get<std::string>();
        data.maxLevel = static_cast<int>(obj["damages"].size());

        int id = data.skillId;
        std::string name = data.skillName;
        _tableById[id] = std::move(data);
        _nameToId[name] = id;
    }

    std::cout << "[SkillTable] Loaded: " << _tableById.size() << " entries" << std::endl;
}

const SkillTableData *SkillTable::Get(int skillId) const {
    auto it = _tableById.find(skillId);
    return (it != _tableById.end()) ? &it->second : nullptr;
}

const SkillTableData *SkillTable::Get(const std::string &skillName) const {
    auto it = _nameToId.find(skillName);
    if (it == _nameToId.end()) return nullptr;
    return Get(it->second);
}
