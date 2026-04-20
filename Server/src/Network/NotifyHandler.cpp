#include "Network/NotifyHandler.h"
#include "Network/Session.h"
#include "GameContext.h"
#include "Protocol.pb.h"

std::unordered_map<PacketId, NotifyHandlerFunc> NotifyHandler::_handlers;

void NotifyHandler::Init()
{
    Register(MSG_RES_ENTER, HandleEnter);
    Register(MSG_RES_LEAVE, HandleLeave);
}

void NotifyHandler::Handle(std::shared_ptr<Session> session, PacketId packetId)
{
    auto it = _handlers.find(packetId);
    if (it != _handlers.end())
        it->second(session);
}

void NotifyHandler::HandleEnter(std::shared_ptr<Session> session)
{
    RES_NOTIFY_ENTER notify;
    notify.set_accountid(session->_accountId);
    GameContext::Instance().sessionManager.BroadCast(MSG_RES_ENTER, notify.SerializeAsString());
}

void NotifyHandler::HandleLeave(std::shared_ptr<Session> session)
{
    RES_NOTIFY_LEAVE notify;
    notify.set_accountid(session->_accountId);
    GameContext::Instance().sessionManager.BroadCast(MSG_RES_LEAVE, notify.SerializeAsString());
}

void NotifyHandler::HandleSkillStart()
{
    RES_NOTIFY_SKILL_START notify;
    GameContext::Instance().sessionManager.BroadCast(MSG_RES_SKILL_START, notify.SerializeAsString());
}

void NotifyHandler::Register(PacketId id, NotifyHandlerFunc handler)
{
    _handlers.insert({id, handler});
}
