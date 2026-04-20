#include "Game/AccountService.h"
#include "GameContext.h"

void AccountService::LoginOrRegister(std::string username,
                                     std::function<void(std::optional<uint32_t>)> callback)
{
    GameContext::Instance().accountRepo.LoginOrRegister(username, callback);
}
