#pragma once

#include <cstdint>
#include <functional>
#include <optional>

class ShopService
{
public:
    void GetShopInfo(uint32_t accountId, std::function<void(uint32_t gold)> callback);

    void BuyItem(uint32_t accountId, uint32_t shopItemId,
                 std::function<void(bool success, uint32_t shopItemId,
                                    uint32_t itemId, uint32_t remainGold)> callback);
};
