#include "../../include/Table/ItemTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void ItemTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[ItemTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        ItemTableData data;
        data.itemId = obj["itemId"].get<int>();
        data.itemName = obj["itemName"].get<std::string>();
        data.itemType = obj["itemType"].get<std::string>();
        data.iconPath = obj["iconPath"].get<std::string>();
        data.effectType = obj["effectType"].get<std::string>();
        data.effectValue = obj["effectValue"].get<int>();
        data.duration = obj["duration"].get<float>();
        _tableById[data.itemId] = std::move(data);
    }

    std::cout << "[ItemTable] Loaded: " << _tableById.size() << " entries" << std::endl;
}

const ItemTableData *ItemTable::Get(int itemId) const {
    auto it = _tableById.find(itemId);
    return (it != _tableById.end()) ? &it->second : nullptr;
}

const std::unordered_map<int, ItemTableData> &ItemTable::GetAll() const {
    return _tableById;
}
