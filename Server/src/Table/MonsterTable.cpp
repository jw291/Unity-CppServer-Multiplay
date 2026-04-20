#include "../../include/Table/MonsterTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void MonsterTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[SkillTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        MonsterTableData data;
        data.monsterId = obj["monsterId"].get<int>();
        data.maxHp = obj["maxHp"].get<int>();
        data.moveSpeed = obj["moveSpeed"].get<float>();
        data.attack = obj["attack"].get<int>();
        data.attackInterval = obj["attackInterval"].get<float>();

        int id = data.monsterId;
        _tableById[id] = std::move(data);
    }

    std::cout << "[MonsterTable] Loaded: " << _tableById.size() << " entries" << std::endl;
}

const MonsterTableData *MonsterTable::Get(int32_t monsterId) const {
    auto it = _tableById.find(monsterId);
    return (it != _tableById.end()) ? &it->second : nullptr;
}

const std::unordered_map<int32_t, MonsterTableData> &MonsterTable::GetAll() const {
    return _tableById;
}
