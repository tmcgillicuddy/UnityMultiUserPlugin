#ifndef FRAMEWORK_STATE_H
#define FRAMEWORK_STATE_H
// RakNet includes
#include "RakNet\RakPeerInterface.h"
#include "RakNet\RakNetTypes.h"
#include "RakNet\MessageIdentifiers.h"

class FrameworkState
{
public:
	int StateFoo(int bar);
	bool StartServer(int maxClients, int portNum, char password[]);
	bool StartClient(char targetIP[], int portNum);
	bool SendData(char data[], int length);
};

#endif // FRAMEWORK_STATE_H