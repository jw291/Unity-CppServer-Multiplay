#pragma once

#include <string>

#include "MonsterTable.h"
#include "SkillTable.h"
#include "ItemTable.h"
#include "ShopTable.h"
#include "DropTable.h"
#include "PlayerTable.h"

class DataManager {
public:
    static DataManager &Instance() {
        static DataManager instance;
        return instance;
    }

    void Init(const std::string &dataRoot);

    const SkillTable &GetSkillTable() const { return _skillTable; }
    const MonsterTable &GetMonsterTable() const { return _monsterTable; }
    const ItemTable &GetItemTable() const { return _itemTable; }
    const ShopTable &GetShopTable() const { return _shopTable; }
    const DropTable &GetDropTable() const { return _dropTable; }
    const PlayerTable &GetPlayerTable() const { return _playerTable; }

private:
    DataManager() = default;

    SkillTable _skillTable;
    MonsterTable _monsterTable;
    ItemTable _itemTable;
    ShopTable _shopTable;
    DropTable _dropTable;
    PlayerTable _playerTable;
};
