# Unity × C++ Server Multiplay

Unity 클라이언트와 C++ (ASIO) 서버로 구현한 실시간 멀티플레이 프로젝트입니다.

## Demo


https://github.com/user-attachments/assets/c9fb22cd-bd92-47b0-9481-bd1c27d3416b



## Tech Stack

**Client** — Unity, C#, Protobuf
**Server** — C++20, ASIO (TCP + UDP), Protobuf, MySQL (X DevAPI)

## Features

- TCP/UDP 혼합 네트워킹 (신뢰성 이벤트는 TCP, 위치 동기화는 UDP)
- 호스트 권위 기반 몬스터 AI (Unity Jobs System Flocking)
- 인벤토리 / 상점 / 스킬 / 화폐 시스템
- 계정 기반 DB 영속화 (로그인, 인벤토리, 스킬, 골드)

## Architecture

- **Game / DB / Network 스레드 분리**: `GameJobQueue`, `DBJobQueue`, ASIO 스레드로 분리 처리
- 스레드 간 작업 전달은 Job Queue를 통해 안전하게 posting
- 역할 기반 클라이언트 초기화 (`SetupHost` / `SetupGuest`)로 런타임 분기 제거

## Structure

- `Client/` — Unity 프로젝트
- `Server/` — C++ 서버 소스 및 빌드 스크립트
