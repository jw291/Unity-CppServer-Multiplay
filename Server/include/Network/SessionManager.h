#pragma once

#include <cstdint>
#include <memory>
#include <unordered_map>
#include <string>

class Session;

class SessionManager
{
public:
    void Enter(std::shared_ptr<Session> session);
    void Leave(std::shared_ptr<Session> session);

    void SetHost(uint32_t accountId) { hostId = accountId; }
    uint32_t GetHostId() const { return hostId; }

    void BroadCast(uint16_t msgId, const std::string& body);
    std::shared_ptr<Session> GetSession(uint32_t accountId);
    bool IsEmpty() const { return sessions.empty(); }

private:
    uint32_t hostId = 0;
    std::unordered_map<uint32_t, std::shared_ptr<Session>> sessions;
};
