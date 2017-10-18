#ifndef FRAMEWORK_STATE_H
#define FRAMEWORK_STATE_H
// RakNet includes
#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"  // MessageID

//Other includes
#include <vector>
#include <string>

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
	RakNet::RakPeerInterface *mpPeer;
	RakNet::Packet *mpPacket;
	std::string mPassword; //Only used if the intial startup settings have a non-null password
	std::vector<ConnectedClient> allConnectedClients;	//Only used when instance is a server
};

#endif // FRAMEWORK_STATE_H