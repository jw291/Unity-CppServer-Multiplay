#pragma once

#include <cstdint>
#include <functional>
#include <unordered_map>

class UdpHandler
{
public:
    static void Init();
    static void Handle(uint16_t msgId, const char* body, uint32_t size);

private:
    static void HandleMonsterMove(const char* body, uint32_t size);
    static void HandlePlayerMove(const char* body, uint32_t size);
    static void HandleUdpRegister(const char* body, uint32_t size);

    using UdpHandlerFunc = std::function<void(const char*, uint32_t)>;
    static std::unordered_map<uint16_t, UdpHandlerFunc> _handlers;
};
