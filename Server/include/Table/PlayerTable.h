#pragma once
#include <cstdint>
#include <string>
#include <unordered_map>

struct PlayerTableData {
    int playerId;
    int maxHp;
    float speed;
    int defense;
};

class PlayerTable {
public:
    void Load(const std::string &path);
    const PlayerTableData *Get(int playerId) const;

private:
    std::unordered_map<int, PlayerTableData> _tableById;
};
