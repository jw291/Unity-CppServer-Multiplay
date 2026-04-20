cmake_minimum_required(VERSION 3.20)
project(Server LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include(FetchContent)

# standalone Asio (header-only)
FetchContent_Declare(
    asio
    GIT_REPOSITORY https://github.com/chriskohlhoff/asio.git
    GIT_TAG        asio-1-30-2
    GIT_SHALLOW    TRUE
)
FetchContent_MakeAvailable(asio)

# nlohmann/json (header-only)
FetchContent_Declare(
    json
    GIT_REPOSITORY https://github.com/nlohmann/json.git
    GIT_TAG        v3.11.3
    GIT_SHALLOW    TRUE
)
FetchContent_MakeAvailable(json)

# Protobuf (vcpkg)
find_package(protobuf CONFIG REQUIRED)

# MySQL Connector/C++ (vcpkg)
find_package(unofficial-mysql-connector-cpp CONFIG REQUIRED)

# protoc 경로
find_program(PROTOC_EXECUTABLE protoc HINTS "${_VCPKG_INSTALLED_DIR}/x64-windows/tools/protobuf")

# .proto → .pb.h/.pb.cc 생성
set(PROTO_FILES ${CMAKE_SOURCE_DIR}/proto/Protocol.proto)
add_custom_command(
    OUTPUT
        ${CMAKE_BINARY_DIR}/Protocol.pb.h
        ${CMAKE_BINARY_DIR}/Protocol.pb.cc
    COMMAND ${PROTOC_EXECUTABLE}
        --proto_path=${CMAKE_SOURCE_DIR}/proto
        --cpp_out=${CMAKE_BINARY_DIR}
        ${PROTO_FILES}
    DEPENDS ${PROTO_FILES}
    COMMENT "Generating protobuf sources"
)

add_executable(Server
    src/main.cpp

    # Network
    src/Network/Server.cpp
    src/Network/Session.cpp
    src/Network/PacketHandler.cpp
    src/Network/SessionManager.cpp
    src/Network/NotifyHandler.cpp
    src/Network/UdpServer.cpp
    src/Network/UdpHandler.cpp

    # DB
    src/DB/DBManager.cpp
    src/DB/AccountRepository.cpp
    src/DB/SkillRepository.cpp
    src/DB/CurrencyRepository.cpp
    src/DB/InventoryRepository.cpp

    # Handler
    src/Handler/CommonHandler.cpp
    src/Handler/SkillHandler.cpp
    src/Handler/CombatHandler.cpp
    src/Handler/ShopHandler.cpp

    # Game
    src/Game/AccountService.cpp
    src/Game/GameService.cpp
    src/Game/SkillService.cpp
    src/Game/MonsterService.cpp
    src/Game/MonsterSpawner.cpp
    src/Game/ShopService.cpp
    src/Game/InventoryService.cpp
    src/Game/PlayerService.cpp

    # Table
    src/Table/DataManager.cpp
    src/Table/SkillTable.cpp
    src/Table/MonsterTable.cpp
    src/Table/ItemTable.cpp
    src/Table/ShopTable.cpp
    src/Table/DropTable.cpp
    src/Table/PlayerTable.cpp

    # Generated
    ${CMAKE_BINARY_DIR}/Protocol.pb.cc

    # Headers
    include/GameContext.h
    include/Network/PacketId.h
    include/Network/Server.h
    include/Network/Session.h
    include/Network/PacketHeader.h
    include/Network/PacketHandler.h
    include/Network/SessionManager.h
    include/Network/NotifyHandler.h
    include/Network/UdpServer.h
    include/Network/UdpHandler.h
    include/DB/DBManager.h
    include/DB/AccountRepository.h
    include/DB/SkillRepository.h
    include/DB/CurrencyRepository.h
    include/DB/InventoryRepository.h
    include/Handler/CommonHandler.h
    include/Handler/SkillHandler.h
    include/Handler/CombatHandler.h
    include/Handler/ShopHandler.h
    include/Game/AccountService.h
    include/Game/GameService.h
    include/Game/SkillService.h
    include/Game/MonsterService.h
    include/Game/MonsterSpawner.h
    include/Game/MonsterState.h
    include/Game/ShopService.h
    include/Game/InventoryService.h
    include/Game/PlayerService.h
    include/Table/DataManager.h
    include/Table/SkillTable.h
    include/Table/MonsterTable.h
    include/Table/ItemTable.h
    include/Table/ShopTable.h
    include/Table/DropTable.h
    include/Table/PlayerTable.h
    include/Util/JobQueue.h
    include/Util/Timer.h

    proto/Protocol.proto
)

target_include_directories(Server PRIVATE
    ${asio_SOURCE_DIR}/asio/include
    ${CMAKE_SOURCE_DIR}/include
    ${CMAKE_BINARY_DIR}
)

target_compile_definitions(Server PRIVATE
    ASIO_STANDALONE
    _WIN32_WINNT=0x0A00
)

target_link_libraries(Server PRIVATE
    protobuf::libprotobuf
    unofficial::mysql-connector-cpp::connector
    nlohmann_json::nlohmann_json
)

if(MSVC)
    target_compile_options(Server PRIVATE /utf-8)
endif()

if(WIN32)
    target_link_libraries(Server PRIVATE ws2_32)
endif()
