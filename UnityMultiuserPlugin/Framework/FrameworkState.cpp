#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum, char password[])
{
	RakNet::SocketDescriptor sd(portNum, 0);	//Calls to start server
	mpPeer->Startup(maxClients, &sd, 1);
	mpPeer->SetMaximumIncomingConnections(maxClients);
	mPassword = password;
	return true;
}

bool FrameworkState::StartClient(char targetIP[], int portNum)
{
	RakNet::SocketDescriptor sd;	//Calls to properly connect to server
	mpPeer->Startup(1, &sd, 1);
	mpPeer->Connect(targetIP, portNum, 0, 0);
	return true;
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
