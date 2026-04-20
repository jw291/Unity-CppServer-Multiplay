#pragma once
#include <cstdint>

struct MonsterState
{
    int32_t seq;
    int32_t monsterId;
    int32_t hp;
    int32_t maxHp;
    float posX;
    float posY;

    bool TakeDamage(int32_t damage)
    {
        hp -= damage;
        return hp <= 0;
    }
};
