#pragma once

#include <functional>
#include <optional>
#include <cstdint>

class CurrencyRepository
{
public:
    void GetGold(uint32_t accountId, std::function<void(uint32_t)> callback);
    void AddGold(uint32_t accountId, uint32_t amount, std::function<void(uint32_t)> callback);
    void SpendGold(uint32_t accountId, uint32_t amount,
                   std::function<void(std::optional<uint32_t>)> callback);

    std::optional<uint32_t> SpendGoldSync(uint32_t accountId, uint32_t amount);
};
