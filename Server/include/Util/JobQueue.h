#pragma once
#include <functional>
#include <mutex>
#include <condition_variable>
#include <vector>

template <typename Tag>

class JobQueue {

public:
    static JobQueue& Instance() {
        static JobQueue instance;
        return instance;
    }
    void Push(const std::function<void()> &job);
    void Execute() ;
private:
    JobQueue() = default;
    std::vector<std::function<void()>> _jobs;
    std::condition_variable _cv;
    std::mutex _mutex;
};

struct GameTag{};
struct DBTag{};
using GameJobQueue = JobQueue<GameTag>;
using DBJobQueue = JobQueue<DBTag>;

template <typename Tag>
void JobQueue<Tag>::Push(const std::function<void()>& job) {
    std::lock_guard<std::mutex> lock(_mutex);
    _jobs.push_back(job);
    _cv.notify_one();
}

template <typename Tag>
void JobQueue<Tag>::Execute() {
    std::vector<std::function<void()>> jobs;
    {
        std::unique_lock<std::mutex> lock(_mutex);
        _cv.wait(lock, [this] { return !_jobs.empty(); });
        jobs.swap(_jobs); // 원래 Queue에서 했는데 Queue 루프 돌때마다 락을 걸어야해서 vector swap으로 한번에 해결.
    }
    for (auto& job : jobs)
        job();
}