#pragma once
#include "FrameworkState.h"

struct ConnectedClient	//Used to store connected client information
{
	char ip[256];
	char userName[256];
};

class ServerState:public FrameworkState
{
public:
	ServerState();
	~ServerState();

	//Init Functions
	bool init(char *targetIP, int portNum, int maxClients);

	//Data Handeling
	bool SendData(int mID, char * data, int length, char * ownerIP);
	char* GetLastPacketIP();


	//Shutdown
	bool cleanup();

	char* UpdateNetwork();

private:
	std::vector<ConnectedClient> allConnectedClients;	//Only used when instance is a server
	RakNet::Packet* lastPacket;
};
