#pragma once

#include <string>
#include <optional>
#include <functional>
#include <cstdint>

class AccountRepository
{
public:
    void LoginOrRegister(const std::string& username,
                         std::function<void(std::optional<uint32_t>)> callback);
};
