#include "DB/DBManager.h"
#include <iostream>

void DBManager::Init(const std::string& host, int port,
                     const std::string& user, const std::string& password,
                     const std::string& schema)
{
    try
    {
        _session = std::make_unique<mysqlx::Session>(host, port, user, password);
        _schema = std::make_unique<mysqlx::Schema>(_session->getSchema(schema));
        std::cout << "[DBManager] Connected to MySQL: " << schema << std::endl;
    }
    catch (const mysqlx::Error& e)
    {
        std::cerr << "[DBManager] Connection failed: " << e.what() << std::endl;
    }
}
