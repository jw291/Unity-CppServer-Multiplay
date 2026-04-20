USE game_server;

-- 화폐 테이블
CREATE TABLE IF NOT EXISTS currencies (
    account_id  INT NOT NULL PRIMARY KEY,
    gold        INT UNSIGNED NOT NULL DEFAULT 0,
    FOREIGN KEY (account_id) REFERENCES accounts(account_id)
);

-- 인벤토리 테이블
CREATE TABLE IF NOT EXISTS inventory (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    account_id  INT NOT NULL,
    slot_index  INT UNSIGNED NOT NULL,
    item_id     INT UNSIGNED NOT NULL,
    quantity    INT UNSIGNED NOT NULL DEFAULT 1,
    UNIQUE KEY uq_account_slot (account_id, slot_index),
    FOREIGN KEY (account_id) REFERENCES accounts(account_id)
);
