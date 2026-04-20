#pragma once
#include <thread>
#include <atomic>
#include <functional>
#include <chrono>

class Timer {
public:
    void Start(float intervalSeconds, std::function<void()> callback) {
        _running = true;
        _thread = std::thread([this, intervalSeconds, callback]() {
            while (_running) {
                std::this_thread::sleep_for(
                    std::chrono::milliseconds((int)(intervalSeconds * 1000)));
                if (_running) callback();
            }
        });
    }

    void Stop() {
        _running = false;
        if (_thread.joinable()) _thread.join();
    }

    ~Timer() { Stop(); }

private:
    std::atomic<bool> _running = false;
    std::thread _thread;
};
