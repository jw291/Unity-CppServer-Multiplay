#include "../../include/Table/DataManager.h"
#include <iostream>

void DataManager::Init(const std::string& dataRoot)
{
    _skillTable.Load(dataRoot + "/SkillTable.json");
    _monsterTable.Load(dataRoot + "/MonsterTable.json");
    _itemTable.Load(dataRoot + "/ItemTable.json");
    _shopTable.Load(dataRoot + "/ShopTable.json");
    _dropTable.Load(dataRoot + "/DropTable.json");
    _playerTable.Load(dataRoot + "/PlayerTable.json");
}
