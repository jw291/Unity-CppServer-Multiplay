#include "Game/GameService.h"
#include "GameContext.h"
#include "Protocol.pb.h"

void GameService::StartGame(uint32_t hostAccountId)
{
    auto& ctx = GameContext::Instance();
    ctx.sessionManager.SetHost(hostAccountId);

    RES_GAME_START res;
    res.set_hostaccountid(hostAccountId);
    ctx.sessionManager.BroadCast(MSG_RES_GAME_START, res.SerializeAsString());

    ctx.skillService.SkillStart();
}
