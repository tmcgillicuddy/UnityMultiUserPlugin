#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum, char password[])
{
	return false;
}

bool FrameworkState::StartClient(char targetIP[], int portNum)
{
	return false;
}

bool FrameworkState::SendData(char data[], int length)
{
	return false;
}
