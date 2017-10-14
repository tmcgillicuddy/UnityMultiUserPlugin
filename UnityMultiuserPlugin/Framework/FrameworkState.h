#ifndef FRAMEWORK_STATE_H
#define FRAMEWORK_STATE_H
// RakNet includes
//#include "RakNet/RakPeerInterface.h"
//#include "RakNet/MessageIdentifiers.h"
//#include "RakNet/RakNetTypes.h"  // MessageID

//Other includes
#include <vector>

struct ConnectedClient	//Used to store connected client information
{
	char ip[256];
	char userName[256];
};

class FrameworkState
{
public:
	//General Functions
	int StateFoo(int bar);
	bool SendData(char data[], int length);

	//Server only Functions
	bool BroadCastData(char data[], int length, char ip[]);
	bool StartServer(int maxClients, int portNum, char password[]);

	//Client Only Functions
	bool StartClient(char targetIP[], int portNum);

private:
	bool isServer;
	std::vector<ConnectedClient> allConnectedClients;	//Only used when instance is a server
};

#endif // FRAMEWORK_STATE_H