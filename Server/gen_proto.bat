@echo off
set PROTOC=C:\Users\jaewonna\vcpkg\installed\x64-windows\tools\protobuf\protoc.exe
set PROTO_DIR=C:\Users\jaewonna\workspace\Server\proto
set OUT_DIR=C:\Users\jaewonna\workspace\Server\build
set CLIENT_OUT_DIR=C:\Users\jaewonna\workspace\Job\Assets\Src

%PROTOC% --proto_path=%PROTO_DIR% --cpp_out=%OUT_DIR% %PROTO_DIR%\Protocol.proto
echo [Server] Protocol.pb.h / Protocol.pb.cc generated

%PROTOC% --proto_path=%PROTO_DIR% --csharp_out=%CLIENT_OUT_DIR% %PROTO_DIR%\Protocol.proto
echo [Client] Protocol.cs generated
