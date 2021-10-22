#include <string>
#include <iostream>
#include <thread>
#include <functional>

std::function<void(const char*, int)> rCallback;
std::thread* t;

extern "C" __declspec(dllexport) int NativeInit()
{
	return 1234;
}
extern "C" __declspec(dllexport) void NativeSend(char* msg, int len)
{
	std::cout << "Msg from c#: ";
	for (size_t i = 0; i < len; i++)
		std::cout << msg[i];
	std::cout << std::endl;
}

void Receve()
{
	while (true)
	{
		std::this_thread::sleep_for(std::chrono::seconds(1));
		std::string msg = "RECEVE UWU";
		rCallback(msg.c_str(), (int)msg.size());
	}
}

extern "C" __declspec(dllexport) void NativeSetReceveCallback(void callback(const char*, int))
{
	rCallback = callback;
	t = new std::thread(Receve);
}

extern "C" __declspec(dllexport) void NativeShutdown()
{
	t->join();
	delete t;
}