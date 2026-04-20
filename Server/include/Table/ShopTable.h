#pragma once
#include <cstdint>
#include <string>
#include <unordered_map>

struct ShopTableData {
    int shopItemId;
    int itemId;
    uint32_t price;
};

class ShopTable {
public:
    void Load(const std::string &path);
    const ShopTableData *Get(int shopItemId) const;
    const std::unordered_map<int, ShopTableData> &GetAll() const;

private:
    std::unordered_map<int, ShopTableData> _tableById;
};
