#include "Network/Server.h"
#include "Network/PacketHandler.h"
#include "Network/UdpServer.h"
#include "Network/UdpHandler.h"
#include "Network/NotifyHandler.h"
#include "DB/DBManager.h"
#include "Table/DataManager.h"
#include "Util/JobQueue.h"
#include "GameContext.h"
#include <iostream>
#include <fstream>
#include <nlohmann/json.hpp>

using json = nlohmann::json;

int main()
{
    try
    {
        std::ifstream configFile("config.json");
        if (!configFile.is_open())
            configFile.open("../config.json");
        if (!configFile.is_open())
        {
            std::cerr << "[Main] config.json not found" << std::endl;
            return 1;
        }

        json config = json::parse(configFile);
        const auto& db = config["db"];

        DBManager::Instance().Init(
            db["host"].get<std::string>(),
            db["port"].get<unsigned int>(),
            db["user"].get<std::string>(),
            db["password"].get<std::string>(),
            db["schema"].get<std::string>()
        );

        DataManager::Instance().Init(config["dataRoot"].get<std::string>());

        PacketHandler::Init();
        UdpHandler::Init();
        NotifyHandler::Init();

        asio::io_context ioContext;

        Server server(ioContext, 7777);
        UdpServer::Instance().Init(ioContext, 7778);

        std::thread gameThread([] {
            while (true)
                GameJobQueue::Instance().Execute();
        });

        std::thread dbThread([] {
            while (true)
                DBJobQueue::Instance().Execute();
        });

        ioContext.run();
        gameThread.join();
        dbThread.join();
    }
    catch (std::exception& e)
    {
        std::cerr << "[Main] Exception: " << e.what() << std::endl;
    }

    return 0;
}
