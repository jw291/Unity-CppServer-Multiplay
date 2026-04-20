#pragma once

#include "MonsterSpawner.h"
#include "MonsterState.h"
#include <cstdint>
#include <unordered_map>

class MonsterService
{
public:
    void StartSpawnLoop(const std::vector<float>& corners);
    void TakeDamage(int32_t monsterSeq, int32_t damage, uint32_t attackerAccountId);
    void PickupDrop(uint32_t dropId, uint32_t accountId);
    int GetMonsterAttack(int32_t monsterSeq) const;

private:
    static constexpr float SPAWN_INTERVAL = 2.0f; //TODO:매직넘버 제거하기

    MonsterSpawner _spawner;
    std::unordered_map<int32_t, MonsterState> _monsters;

    uint32_t _dropIdCounter = 0;
    std::unordered_map<uint32_t, uint32_t> _pendingDrops;

    void BroadcastSpawn(std::vector<MonsterSpawnInfo> infos);
    void ProcessDrop(const MonsterState& state);
};
