@echo off
start "Server" powershell -NoExit -Command "& 'C:\Users\jaewonna\workspace\Server\build\Release\Server.exe'"
start "Client" powershell -NoExit -Command "telnet localhost 7777"
