#pragma once
#include <cstdint>
#include <string>
#include <unordered_map>

struct ItemTableData {
    int itemId;
    std::string itemName;
    std::string itemType;
    std::string iconPath;
    std::string effectType;
    int effectValue;
    float duration;
};

class ItemTable {
public:
    void Load(const std::string &path);
    const ItemTableData *Get(int itemId) const;
    const std::unordered_map<int, ItemTableData> &GetAll() const;

private:
    std::unordered_map<int, ItemTableData> _tableById;
};
