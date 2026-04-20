#include "Handler/CommonHandler.h"
#include "Network/Session.h"
#include "GameContext.h"
#include "Protocol.pb.h"
#include <iostream>

namespace CommonHandler
{

static void HandleLogin(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_LOGIN req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_LOGIN" << std::endl;
        return;
    }

    auto username = req.username();
    GameContext::Instance().accountService.LoginOrRegister(username,
        [session, username](std::optional<uint32_t> accountId)
        {
            RES_LOGIN res;
            if (!accountId.has_value())
            {
                res.set_success(false);
                session->Send(MSG_RES_LOGIN, res.SerializeAsString());
                return;
            }

            res.set_success(true);
            res.set_accountid(*accountId);
            res.set_username(username);
            session->Send(MSG_RES_LOGIN, res.SerializeAsString());

            session->_accountId = *accountId;
            GameContext::Instance().sessionManager.Enter(session);
            GameContext::Instance().playerService.InitPlayer(*accountId);
        });
}

static void HandleEcho(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_ECHO req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_ECHO" << std::endl;
        return;
    }

    RES_ECHO res;
    res.set_text("[Echo] " + req.text());
    session->Send(MSG_RES_ECHO, res.SerializeAsString());
}

static void HandlePingPong(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_PING req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_PING" << std::endl;
        return;
    }

    RES_PONG res;
    res.set_timestamp(req.timestamp());
    session->Send(MSG_RES_PONG, res.SerializeAsString());
}

static void HandleGameStart(std::shared_ptr<Session> session, const char* body, uint32_t size)
{
    REQ_GAME_START req;
    if (!req.ParseFromArray(body, size))
    {
        std::cout << "[Handler] Failed to parse REQ_GAME_START" << std::endl;
        return;
    }

    GameContext::Instance().gameService.StartGame(req.hostaccountid());
}

void Register(std::unordered_map<PacketId, PacketHandlerFunc>& handlers)
{
    handlers[MSG_REQ_LOGIN] = HandleLogin;
    handlers[MSG_REQ_ECHO] = HandleEcho;
    handlers[MSG_REQ_PING] = HandlePingPong;
    handlers[MSG_REQ_GAME_START] = HandleGameStart;
}

} 
