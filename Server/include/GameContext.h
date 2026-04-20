#pragma once

#include "Network/PacketId.h"
#include "Network/SessionManager.h"
#include "Game/AccountService.h"
#include "Game/GameService.h"
#include "Game/MonsterService.h"
#include "Game/PlayerService.h"
#include "Game/SkillService.h"
#include "Game/ShopService.h"
#include "Game/InventoryService.h"
#include "DB/AccountRepository.h"
#include "DB/SkillRepository.h"
#include "DB/CurrencyRepository.h"
#include "DB/InventoryRepository.h"

//아무리 생각해도 이걸 다 DI로 하면 비즈니스 로직이 많아졌을 때 초기화 코드가 너무 많아짐
//싱글톤으로 하자니 너무 우후죽순으로 늘어남 무엇보다, Zone/Room 구조에서 인스턴스를 못나눔
//그래서, Context단위로 묶어서 Zone/Room 구조에서도 인스턴스를 나눌 수 있도록 하고 싱글턴 코드들의 존재유무를 가독성있게 식별할 수 있도록 함. 더 좋은 방법이 있나?
class GameContext
{
public:
    static GameContext& Instance()
    {
        static GameContext instance;
        return instance;
    }

    // DB관련
    AccountRepository accountRepo;
    SkillRepository skillRepo;
    CurrencyRepository currencyRepo;
    InventoryRepository inventoryRepo;

    // Network
    SessionManager sessionManager;

    // Services
    AccountService accountService;
    GameService gameService;
    MonsterService monsterService;
    PlayerService playerService;
    SkillService skillService;
    ShopService shopService;
    InventoryService inventoryService;

private:
    GameContext() = default;
};
