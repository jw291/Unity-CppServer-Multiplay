#include "../../include/Table/PlayerTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void PlayerTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[PlayerTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        PlayerTableData data;
        data.playerId = obj["playerId"].get<int>();
        data.maxHp = obj["maxHp"].get<int>();
        data.speed = obj["speed"].get<float>();
        data.defense = obj["defense"].get<int>();
        _tableById[data.playerId] = std::move(data);
    }

    std::cout << "[PlayerTable] Loaded: " << _tableById.size() << " entries" << std::endl;
}

const PlayerTableData *PlayerTable::Get(int playerId) const {
    auto it = _tableById.find(playerId);
    return (it != _tableById.end()) ? &it->second : nullptr;
}
