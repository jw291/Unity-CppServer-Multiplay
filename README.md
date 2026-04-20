# Unity × C++ Server Multiplay

Unity 클라이언트와 C++ (ASIO) 서버로 구현한 실시간 멀티플레이 프로젝트입니다.
클라이언트 개발자로서, ASIO 서버와 C++을 개인적으로 공부하기 위해 시작했습니다.
클라이언트 코드는 C++ 서버를 돌리기 위해 간단하게 구현했고 서버 코드에 집중했습니다.

## Demo
https://github.com/user-attachments/assets/c9fb22cd-bd92-47b0-9481-bd1c27d3416b



## Server 소개

### 빌드 및 실행
- 환경: Windows, MSVC, CMake 3.20+, vcpkg (protobuf, mysql-connector-cpp)
- 의존성 fetch: asio 1.30.2, nlohmann/json 3.11.3
- 빌드 순서 / config.json 작성법 (config.json.example 참고) / DB 스키마 (sql/)

### 기술 스택
- 네트워크: ASIO standalone 비동기 I/O, TCP + UDP 이중 전송
- 직렬화: Protocol Buffers, 6바이트 헤더 [msgId:u16 | size:u32] 빅엔디안
- DB: MySQL Connector/C++ X DevAPI, mysqlx::expr() 활용한 원자적 업데이트
- 스레드 모델: I/O 스레드 / Game 스레드 / DB 스레드 3분리 + JobQueue<Tag> 템플릿으로 스레드 간 작업 전달 (Queue Lock 방식이 아님 단일 Lock과 vector Swap 방식으로 최적화)
- 설정/데이터: nlohmann/json 기반 config, JSON 데이터 테이블 (Monster/Item/Shop/Drop/Skill/Player)

### 아키텍처 및 설계
- 순환 참조 최소화를 위한 레이어드: Network → Handler(도메인별 라우팅) → Service(게임 로직) → Repository(DB 추상화) → State(도메인 데이터+행동)
- Handler 도메인 분리: Common / Skill / Combat / Shop ..
- Repository 패턴: Account / Skill / Currency / Inventory — DBManager는 커넥션만 보유
- GameContext (Service Locator): 모든 Service/Repository를 한 싱글톤이 보유, Service 간 싱글톤 난립 방지 및 Room/Zone 구조로 확장 가능한 구조 구현
- 행동을 State에 귀속: MonsterState::TakeDamage, PlayerState::Heal 등 데이터+관련 로직 같은 곳


