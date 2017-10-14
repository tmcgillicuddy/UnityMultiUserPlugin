#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum, char password[])
{
	//TODO: Start a server with the given maxClients, portNum and password

	return false;
}

bool FrameworkState::StartClient(char targetIP[], int portNum)
{
	//TODO: Start the client with the given ip and port num

	return false;
}

bool FrameworkState::SendData(char data[], int length)
{
	//TODO: Either send the data to all connected clients or to the server

	return false;
}

bool FrameworkState::BroadCastData(char data[], int length, char ip[])
{
	//TODO: Relay data to all connected client EXCEPT the client with the given ip, prevents ghosting

	return false;
}
