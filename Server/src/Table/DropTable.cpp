#include "../../include/Table/DropTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void DropTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[DropTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        DropTableData data;
        data.monsterId = obj["monsterId"].get<int>();
        data.minGold = obj["minGold"].get<int>();
        data.maxGold = obj["maxGold"].get<int>();
        _tableByMonsterId[data.monsterId] = data;
    }

    std::cout << "[DropTable] Loaded: " << _tableByMonsterId.size() << " entries" << std::endl;
}

const DropTableData *DropTable::Get(int monsterId) const {
    auto it = _tableByMonsterId.find(monsterId);
    return (it != _tableByMonsterId.end()) ? &it->second : nullptr;
}
