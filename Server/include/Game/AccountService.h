#pragma once

#include <functional>
#include <optional>
#include <string>
#include <cstdint>

class AccountService
{
public:
    void LoginOrRegister(std::string username,
                         std::function<void(std::optional<uint32_t>)> callback);
};
