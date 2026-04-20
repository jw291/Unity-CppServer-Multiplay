#pragma once

#include <vector>
#include <unordered_map>
#include <memory>
#include "MonsterState.h"
#include "../Util/Timer.h"
#include "Protocol.pb.h"

class MonsterSpawner
{
public:
    void MakeSpawnInfos(std::unordered_map<int32_t, MonsterState>& monsters);
    void StartSpawnLoop(const std::vector<float>& spawnCorners,
        std::unordered_map<int32_t, MonsterState>& monsters,
        float spawnInterval);
    void SetSpawnCallBack(const std::function<void(std::vector<MonsterSpawnInfo>)> &callback) {
        _spawnCallBack = callback;
    }
    void Erase(int32_t monsterSeq) {
        _spawnMonsters.erase(monsterSeq);
    }
private:
    Timer _timer;
    std::vector<float> _corners;
    std::unordered_map<int32_t, MonsterSpawnInfo> _spawnMonsters;
    std::function<void(std::vector<MonsterSpawnInfo>)> _spawnCallBack;
    int32_t _spawnSequence = 0;
    bool _running = false;

    std::vector<float> GetRandomPosition();
};
