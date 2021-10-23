#include <string>
#include <iostream>
#include <thread>
#include <functional>

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>

// Need to link with Ws2_32.lib, Mswsock.lib, and Advapi32.lib
#pragma comment (lib, "Ws2_32.lib")
#pragma comment (lib, "Mswsock.lib")
#pragma comment (lib, "AdvApi32.lib")

#define DEFAULT_BUFLEN 512
#define DEFAULT_IP "87.248.13.40"
#define DEFAULT_PORT "27015"

WSADATA wsaData;
SOCKET ConnectSocket = INVALID_SOCKET;
struct addrinfo* result = NULL,	*ptr = NULL, hints;

std::function<void(const char*, int)> rCallback;
std::thread* t;
bool run = true;

extern "C" __declspec(dllexport) int NativeInit()
{

	int iResult;
	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed with error: %d\n", iResult);
		return 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	// Resolve the server address and port
	iResult = getaddrinfo(DEFAULT_IP, DEFAULT_PORT, &hints, &result);
	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		return 1;
	}
	// Attempt to connect to an address until one succeeds
	for (ptr = result; ptr != NULL; ptr = ptr->ai_next) {

		// Create a SOCKET for connecting to server
		ConnectSocket = socket(ptr->ai_family, ptr->ai_socktype,
			ptr->ai_protocol);
		if (ConnectSocket == INVALID_SOCKET) {
			printf("socket failed with error: %ld\n", WSAGetLastError());
			WSACleanup();
			return 1;
		}

		// Connect to server.
		iResult = connect(ConnectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);
		if (iResult == SOCKET_ERROR) {
			closesocket(ConnectSocket);
			ConnectSocket = INVALID_SOCKET;
			continue;
		}
		break;
	}

	freeaddrinfo(result);

	if (ConnectSocket == INVALID_SOCKET) {
		printf("Unable to connect to server!\n");
		WSACleanup();
		return 1;
	}
	return 0;
}
extern "C" __declspec(dllexport) void NativeShutdown();
extern "C" __declspec(dllexport) void NativeSend(char* msg, int len)
{
	int iResult;
	std::cout << "Sending: ";
	for (size_t i = 0; i < len; i++)
		std::cout << msg[i];
	std::cout << std::endl;
	iResult = send(ConnectSocket, msg, len, 0);
	if (iResult == SOCKET_ERROR) {
		printf("send failed with error: %d\n", WSAGetLastError());
		NativeShutdown();
	}
}

void Receve()
{
	//while (run)
	//{
	//	std::this_thread::sleep_for(std::chrono::seconds(2));
	//	std::string msg = "RECEVE UWU";
	//	rCallback(msg.c_str(), (int)msg.size());
	//}

	char recvbuf[DEFAULT_BUFLEN];
	int recvbuflen = DEFAULT_BUFLEN;
	int iResult;
	// Receive until the peer closes the connection
	do {
		memset(recvbuf, '\0', recvbuflen);
		iResult = recv(ConnectSocket, recvbuf, recvbuflen, 0);
		if (iResult >= 0)
		{
			// printf("Bytes received: %d\n", iResult);
			rCallback(recvbuf, recvbuflen);
		}
		else
			printf("recv failed with error: %d\n", WSAGetLastError());

	} while (iResult > 0 && run);
	printf("Connection closed\n");
}

extern "C" __declspec(dllexport) void NativeSetReceveCallback(void callback(const char*, int))
{
	rCallback = callback;
	t = new std::thread(Receve);
}

extern "C" __declspec(dllexport) void NativeShutdown()
{
	run = false;
	closesocket(ConnectSocket);
	t->join();
	delete t;
	shutdown(ConnectSocket, SD_SEND);
	WSACleanup();
	std::cout << "Cpp cleanup\n";
}