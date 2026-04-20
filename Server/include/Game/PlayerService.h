#pragma once

#include <cstdint>
#include <unordered_map>
#include <algorithm>
#include <utility>

struct PlayerState
{
    uint32_t accountId;
    int hp;
    int maxHp;
    int defense;

    int TakeDamage(int attack)
    {
        int damage = std::max(1, attack - defense);
        hp = std::max(0, hp - damage);
        return damage;
    }

    void Heal(int amount)
    {
        hp = std::min(hp + amount, maxHp);
    }
};

class PlayerService
{
public:
    void InitPlayer(uint32_t accountId);
    std::pair<int, int> TakeDamage(uint32_t accountId, int monsterAttack);
    void HealHp(uint32_t accountId, int amount);

private:
    std::unordered_map<uint32_t, PlayerState> players;
};
