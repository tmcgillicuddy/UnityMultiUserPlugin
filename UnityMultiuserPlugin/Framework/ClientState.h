#pragma once
#include "FrameworkState.h"

class ClientState : public FrameworkState
{
public:
	ClientState();
	~ClientState();

	//Init Functions
	bool init(char *targetIP, int portNum, int maxClients);

	//Data Handeling
	bool SendData(char * data, int length, char * ownerIP);
	char* GetLastPacketIP();

	//Shutdown
	bool cleanup();


	char* UpdateNetwork();

private:
	std::string mServerIP;

};

