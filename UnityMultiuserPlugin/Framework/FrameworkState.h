#ifndef FRAMEWORK_STATE_H
#define FRAMEWORK_STATE_H
#define _ITERATOR_DEBUG_LEVEL 0	//For compiling in release mode properly
// RakNet includes
#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"  // MessageID

//Other includes
#include <vector>
#include <string>
#include "LogWriter.h"

class LogWriter;
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

	//Update the network loop
	int UpdateNetwork();

	//TODO: Need a way to either call a function in unity to deserlize a char array

	//TODO: Find a way to send char array to c++ and from c++

	//TODO: Find a way to debug the DLL (One option is a log writer that writes out each step as it happens)
	void resetLogger();
	void writeToLogger(std::string message);

private:
	bool isServer;
	RakNet::RakPeerInterface *mpPeer;
	RakNet::Packet *mpPacket;
	std::string mPassword; //Only used if the intial startup settings have a non-null password
	std::vector<ConnectedClient> allConnectedClients;	//Only used when instance is a server
	LogWriter pLogger;
};

#endif // FRAMEWORK_STATE_H