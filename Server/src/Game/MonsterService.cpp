#include "Game/MonsterService.h"
#include "GameContext.h"
#include "Network/Session.h"
#include "Table/DataManager.h"
#include "DB/CurrencyRepository.h"
#include "Protocol.pb.h"
#include <iostream>
#include <random>

void MonsterService::StartSpawnLoop(const std::vector<float>& corners)
{
    _spawner.SetSpawnCallBack([this](const std::vector<MonsterSpawnInfo>& infos) {
        BroadcastSpawn(infos);
    });
    _spawner.StartSpawnLoop(corners, _monsters, SPAWN_INTERVAL);
}

void MonsterService::TakeDamage(int32_t monsterSeq, int32_t damage, uint32_t attackerAccountId)
{
    auto it = _monsters.find(monsterSeq);
    if (it == _monsters.end())
        return;

    auto& monster = it->second;
    bool dead = monster.TakeDamage(damage);

    if (dead)
    {
        RES_MONSTER_DEAD deadMsg;
        deadMsg.set_monsterseq(monsterSeq);
        GameContext::Instance().sessionManager.BroadCast(
            MSG_RES_MONSTER_DEAD, deadMsg.SerializeAsString());

        MonsterState deadState = monster;
        _monsters.erase(it);
        _spawner.Erase(monsterSeq);
        std::cout << "[MonsterService] Monster dead: seq=" << monsterSeq << std::endl;

        ProcessDrop(deadState);
    }
    else
    {
        RES_MONSTER_HIT hit;
        hit.set_monsterseq(monsterSeq);
        hit.set_remainhp(monster.hp);
        GameContext::Instance().sessionManager.BroadCast(
            MSG_RES_MONSTER_HIT, hit.SerializeAsString());
    }
}

void MonsterService::ProcessDrop(const MonsterState& state)
{
    const auto* dropData = DataManager::Instance().GetDropTable().Get(state.monsterId);
    if (!dropData)
        return;

    static std::mt19937 rng(std::random_device{}());
    std::uniform_int_distribution<int> goldDist(dropData->minGold, dropData->maxGold);

    uint32_t gold = (uint32_t)goldDist(rng);
    uint32_t dropId = ++_dropIdCounter;
    _pendingDrops[dropId] = gold;

    RES_MONSTER_DROP res;
    res.set_monsterseq(state.seq);
    res.set_dropid(dropId);
    res.set_gold(gold);
    res.set_posx(state.posX);
    res.set_posy(state.posY);
    GameContext::Instance().sessionManager.BroadCast(
        MSG_RES_MONSTER_DROP, res.SerializeAsString());

    std::cout << "[MonsterService] Drop created: dropId=" << dropId
              << " gold=" << gold << std::endl;
}

void MonsterService::PickupDrop(uint32_t dropId, uint32_t accountId)
{
    auto it = _pendingDrops.find(dropId);
    if (it == _pendingDrops.end())
        return;

    uint32_t gold = it->second;
    _pendingDrops.erase(it);

    RES_PICKUP_DROP pickupRes;
    pickupRes.set_dropid(dropId);
    GameContext::Instance().sessionManager.BroadCast(
        MSG_RES_PICKUP_DROP, pickupRes.SerializeAsString());

    GameContext::Instance().currencyRepo.AddGold(accountId, gold,
        [accountId, gold](uint32_t newGold)
        {
            auto session = GameContext::Instance().sessionManager.GetSession(accountId);
            if (!session)
                return;

            RES_CURRENCY_UPDATE res;
            res.set_gold(newGold);
            session->Send(MSG_RES_CURRENCY_UPDATE, res.SerializeAsString());

            std::cout << "[MonsterService] Pickup: accountId=" << accountId
                      << " gold+" << gold << " total=" << newGold << std::endl;
        });
}

int MonsterService::GetMonsterAttack(int32_t monsterSeq) const
{
    auto it = _monsters.find(monsterSeq);
    if (it == _monsters.end())
        return -1;

    const auto* data = DataManager::Instance().GetMonsterTable().Get(it->second.monsterId);
    return data ? data->attack : -1;
}

void MonsterService::BroadcastSpawn(std::vector<MonsterSpawnInfo> infos)
{
    RES_MONSTER_SPAWN res;
    for (auto& info : infos)
        *res.add_spawninfo() = info;

    GameContext::Instance().sessionManager.BroadCast(
        MSG_RES_MONSTER_SPAWN, res.SerializeAsString());
}
