#pragma once
#include <cstdint>
#include <string>
#include <unordered_map>

struct MonsterTableData {
    int monsterId;
    int maxHp;
    float moveSpeed;
    int attack;
    float attackInterval;
};

class MonsterTable {
public:
    void Load(const std::string &path);
    const MonsterTableData *Get(int32_t monsterId) const;
    const std::unordered_map<int32_t, MonsterTableData> &GetAll() const;

private:
    std::unordered_map<int32_t, MonsterTableData> _tableById;
};
