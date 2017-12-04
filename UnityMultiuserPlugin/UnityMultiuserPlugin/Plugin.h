#ifndef PLUGIN_H
#define PLUGIN_H

// actual plugin interface
#include "lib.h"


// anything following this will be C linkage
#ifdef __cplusplus
extern "C"
{
#endif // _cplusplus

	// shutdown
	MULTIUSER_PLUGIN_SYMBOL int Shutdown();

	MULTIUSER_PLUGIN_SYMBOL int StartServer(char* password,int maxClients, int portNum);

	MULTIUSER_PLUGIN_SYMBOL int StartClient(char* targetIP, int portNum);

	MULTIUSER_PLUGIN_SYMBOL int SendData(char* data, int length, char* ownerIP);

	MULTIUSER_PLUGIN_SYMBOL int SendMessageData(char * data, int length, char * ownerIP);

	MULTIUSER_PLUGIN_SYMBOL char* GetData();

	MULTIUSER_PLUGIN_SYMBOL char* GetLastPacketIP();
#ifdef __cplusplus
}
#endif // __cplusplus

#endif // PLUGIN_H
