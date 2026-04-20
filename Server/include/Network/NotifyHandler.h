#pragma once

#include <cstdint>
#include <memory>
#include <functional>
#include <unordered_map>
#include "PacketId.h"

class Session;

using NotifyHandlerFunc = std::function<void(std::shared_ptr<Session>)>;

class NotifyHandler
{
public:
    static void Init();
    static void Handle(std::shared_ptr<Session> session, PacketId packetId);

private:
    static void HandleEnter(std::shared_ptr<Session> session);
    static void HandleLeave(std::shared_ptr<Session> session);
    static void HandleSkillStart();

    static void Register(PacketId id, NotifyHandlerFunc handler);
    static std::unordered_map<PacketId, NotifyHandlerFunc> _handlers;
};
