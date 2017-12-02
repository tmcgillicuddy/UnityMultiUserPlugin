#ifndef FRAMEWORK_STATE_H
#define FRAMEWORK_STATE_H
// RakNet includes
#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"  // MessageID

//Other includes
#include <vector>
#include <string>
#include "LogWriter.h"

class LogWriter;

#pragma pack(push,1)
struct dataBuffer
{
	char messageID = 136;	//Should be the game object update message
	char buffer[512];
};

#pragma pack(pop)

class FrameworkState abstract
{
public:
	//Startup Functions
	virtual bool init(char *targetIP, int portNum, int maxClients) = 0;

	//Data Handeling
	virtual bool SendData(char * data, int length, char * ownerIP) = 0;
	virtual bool SendMessageData(char * data, int length, char * ownerIP) = 0;
	virtual char* UpdateNetwork() = 0;

	void resetLogger();
	void writeToLogger(std::string message);
	void drawLineOnLogger() { pLogger.drawLine(); };

protected:
	RakNet::RakPeerInterface *mpPeer = RakNet::RakPeerInterface::GetInstance();
	std::string mPassword; //Only used if the intial startup settings have a non-null password
	LogWriter pLogger;
};

#endif // FRAMEWORK_STATE_H