#include "Network/SessionManager.h"
#include "Network/Session.h"
#include "Network/NotifyHandler.h"
#include "Protocol.pb.h"

void SessionManager::Enter(std::shared_ptr<Session> session)
{
    auto it = sessions.find(session->_accountId);
    if (it != sessions.end())
        it->second->Close();
    else
        sessions.insert({session->_accountId, session});

    for (auto& [id, otherSession] : sessions)
    {
        if (id == session->_accountId)
            continue;

        RES_NOTIFY_ENTER notify;
        notify.set_accountid(id);
        session->Send(MSG_RES_ENTER, notify.SerializeAsString());
    }

    for (auto& [id, otherSession] : sessions)
    {
        if (id == session->_accountId)
            continue;

        RES_NOTIFY_ENTER notify;
        notify.set_accountid(session->_accountId);
        otherSession->Send(MSG_RES_ENTER, notify.SerializeAsString());
    }
}

void SessionManager::Leave(std::shared_ptr<Session> session)
{
    sessions.erase(session->_accountId);
    NotifyHandler::Handle(session, MSG_RES_LEAVE);
}

void SessionManager::BroadCast(uint16_t msgId, const std::string& body)
{
    for (auto& [id, session] : sessions)
        session->Send(msgId, body);
}

std::shared_ptr<Session> SessionManager::GetSession(uint32_t accountId)
{
    auto it = sessions.find(accountId);
    return (it != sessions.end()) ? it->second : nullptr;
}
