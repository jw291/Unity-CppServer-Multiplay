#include "Game/MonsterSpawner.h"
#include "Table/DataManager.h"
#include "Util/JobQueue.h"
#include <iostream>
#include <random>

void MonsterSpawner::StartSpawnLoop(const std::vector<float>& corners,
    std::unordered_map<int32_t, MonsterState>& monsters,
    float spawnInterval)
{
    this->_corners = corners;

    if (_running)
        return;
    _running = true;

    // #1 fix: push MakeSpawnInfos to GameJobQueue (avoid race with game thread)
    _timer.Start(spawnInterval, [this, &monsters]() {
        GameJobQueue::Instance().Push([this, &monsters]() {
            MakeSpawnInfos(monsters);
        });
    });
    std::cout << "[MonsterSpawnManager] Spawn loop started" << std::endl;
}

void MonsterSpawner::MakeSpawnInfos(std::unordered_map<int32_t, MonsterState>& monsters)
{
    const auto& monsterTableDatas = DataManager::Instance().GetMonsterTable();
    auto& dataAll = monsterTableDatas.GetAll();
    std::vector<int> monsterIds;
    monsterIds.reserve(dataAll.size());

    for (const auto& [id, data] : dataAll)
        monsterIds.push_back(id);

    static std::random_device rd;
    static std::mt19937 gen(rd());
    std::uniform_int_distribution<size_t> dist(0, monsterIds.size() - 1);

    std::vector<MonsterSpawnInfo> spawnInfos;
    for (auto i = 0; i < 5; i++)
    {
        int randomId = monsterIds[dist(gen)];
        const MonsterTableData* randomData = monsterTableDatas.Get(randomId);
        if (randomData == nullptr)
            continue;

        auto pos = GetRandomPosition();

        MonsterSpawnInfo info;
        info.set_monsterid(randomData->monsterId);
        info.set_monsterseq(_spawnSequence);
        info.set_posx(pos[0]);
        info.set_posy(pos[1]);
        _spawnMonsters[_spawnSequence] = info;

        MonsterState state;
        state.seq = _spawnSequence;
        state.monsterId = randomData->monsterId;
        state.hp = randomData->maxHp;
        state.maxHp = randomData->maxHp;
        state.posX = pos[0];
        state.posY = pos[1];
        monsters[_spawnSequence] = state;

        spawnInfos.push_back(info);
        _spawnSequence++;
    }

    if (_spawnCallBack)
        _spawnCallBack(spawnInfos);
}

std::vector<float> MonsterSpawner::GetRandomPosition()
{
    static std::random_device rd;
    static std::mt19937 gen(rd());

    std::uniform_real_distribution<float> distX(_corners[0], _corners[1]);
    std::uniform_real_distribution<float> distY(_corners[2], _corners[3]);

    return {distX(gen), distY(gen)};
}
