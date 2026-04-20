#include "../../include/Table/ShopTable.h"
#include <nlohmann/json.hpp>
#include <fstream>
#include <iostream>
using json = nlohmann::json;

void ShopTable::Load(const std::string &path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[ShopTable] Failed to open: " << path << std::endl;
        return;
    }

    json arr = json::parse(file);
    for (auto &obj: arr) {
        ShopTableData data;
        data.shopItemId = obj["shopItemId"].get<int>();
        data.itemId = obj["itemId"].get<int>();
        data.price = obj["price"].get<uint32_t>();
        _tableById[data.shopItemId] = std::move(data);
    }

    std::cout << "[ShopTable] Loaded: " << _tableById.size() << " entries" << std::endl;
}

const ShopTableData *ShopTable::Get(int shopItemId) const {
    auto it = _tableById.find(shopItemId);
    return (it != _tableById.end()) ? &it->second : nullptr;
}

const std::unordered_map<int, ShopTableData> &ShopTable::GetAll() const {
    return _tableById;
}
