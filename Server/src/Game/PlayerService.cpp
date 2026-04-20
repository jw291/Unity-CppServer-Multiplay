#include "Game/PlayerService.h"
#include "Table/DataManager.h"
#include <iostream>

void PlayerService::InitPlayer(uint32_t accountId)
{
    const auto* data = DataManager::Instance().GetPlayerTable().Get(1);
    if (!data)
    {
        std::cerr << "[PlayerService] PlayerTable playerId=1 not found\n";
        return;
    }

    PlayerState state;
    state.accountId = accountId;
    state.maxHp = data->maxHp;
    state.hp = data->maxHp;
    state.defense = data->defense;
    players[accountId] = state;

    std::cout << "[PlayerService] InitPlayer accountId=" << accountId
              << " hp=" << state.hp << " def=" << state.defense << "\n";
}

std::pair<int, int> PlayerService::TakeDamage(uint32_t accountId, int monsterAttack)
{
    auto it = players.find(accountId);
    if (it == players.end())
        return {0, 0};

    auto& state = it->second;
    int damage = state.TakeDamage(monsterAttack);

    std::cout << "[PlayerService] TakeDamage accountId=" << accountId
              << " damage=" << damage << " remainHp=" << state.hp << "\n";

    return {damage, state.hp};
}

void PlayerService::HealHp(uint32_t accountId, int amount)
{
    auto it = players.find(accountId);
    if (it == players.end())
        return;

    it->second.Heal(amount);
}
