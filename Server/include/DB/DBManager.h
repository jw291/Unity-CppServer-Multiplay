#pragma once

#include <string>
#include <memory>
#include <mysqlx/xdevapi.h>

class DBManager
{
public:
    static DBManager& Instance()
    {
        static DBManager instance;
        return instance;
    }

    void Init(const std::string& host, int port,
              const std::string& user, const std::string& password,
              const std::string& schema);

    mysqlx::Schema& GetSchema() { return *_schema; }
    mysqlx::Session& GetSession() { return *_session; }

private:
    DBManager() = default;
    std::unique_ptr<mysqlx::Session> _session;
    std::unique_ptr<mysqlx::Schema> _schema;
};
