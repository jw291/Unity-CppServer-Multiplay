#include "Handler/CombatHandler.h"
#include "Network/Session.h"
#include "GameContext.h"
#include "Protocol.pb.h"
#include <iostream>

namespace CombatHandler
{

static void HandleMonsterSpawn(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_MONSTER_SPAWN req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_MONSTER_SPAWN" << std::endl;
        return;
    }

    const auto& corners = req.corners();
    std::vector<float> cornerVector(corners.begin(), corners.end());
    GameContext::Instance().monsterService.StartSpawnLoop(cornerVector);
}

static void HandleMonsterHit(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_MONSTER_HIT req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_MONSTER_HIT" << std::endl;
        return;
    }

    GameContext::Instance().monsterService.TakeDamage(
        req.monsterseq(), req.damage(), session->_accountId);
}

static void HandlePickupDrop(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_PICKUP_DROP req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_PICKUP_DROP" << std::endl;
        return;
    }

    GameContext::Instance().monsterService.PickupDrop(req.dropid(), session->_accountId);
}

static void HandlePlayerHit(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_PLAYER_HIT req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_PLAYER_HIT" << std::endl;
        return;
    }

    uint32_t accountId = req.accountid();
    int32_t monsterSeq = req.monsterseq();

    int monsterAttack = GameContext::Instance().monsterService.GetMonsterAttack(monsterSeq);
    if (monsterAttack < 0)
        return;

    auto [damage, remainHp] = GameContext::Instance().playerService.TakeDamage(accountId, monsterAttack);

    RES_PLAYER_HIT res;
    res.set_accountid(accountId);
    res.set_damage(damage);
    res.set_remainhp(remainHp);
    GameContext::Instance().sessionManager.BroadCast(MSG_RES_PLAYER_HIT, res.SerializeAsString());
}

void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers)
{
    handlers[MSG_REQ_MONSTER_SPAWN] = HandleMonsterSpawn;
    handlers[MSG_REQ_MONSTER_HIT] = HandleMonsterHit;
    handlers[MSG_REQ_PICKUP_DROP] = HandlePickupDrop;
    handlers[MSG_REQ_PLAYER_HIT] = HandlePlayerHit;
}

} // namespace CombatHandler
