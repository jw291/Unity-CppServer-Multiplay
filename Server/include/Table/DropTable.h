#pragma once
#include <cstdint>
#include <string>
#include <unordered_map>

struct DropTableData
{
    int monsterId;
    int minGold;
    int maxGold;
};

class DropTable
{
public:
    void Load(const std::string& path);
    const DropTableData* Get(int monsterId) const;
private:
    std::unordered_map<int, DropTableData> _tableByMonsterId;
};
