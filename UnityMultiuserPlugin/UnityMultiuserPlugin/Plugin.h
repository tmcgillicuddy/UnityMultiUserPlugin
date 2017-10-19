#ifndef PLUGIN_H
#define PLUGIN_H
// actual plugin interface
#include "lib.h"


// anything following this will be C linkage
#ifdef __cplusplus
extern "C"
{
#endif // _cplusplus

	// startup
	MULTIUSER_PLUGIN_SYMBOL int Startup();

	// shutdown
	MULTIUSER_PLUGIN_SYMBOL int Shutdown();

	// c style declaration for framework wrappers
	MULTIUSER_PLUGIN_SYMBOL int Foo(int bar);

	MULTIUSER_PLUGIN_SYMBOL int StartServer(int maxClients, int portNum);

	MULTIUSER_PLUGIN_SYMBOL int StartClient(char * targetIP, int portNum);

	MULTIUSER_PLUGIN_SYMBOL int SendData(char data[], int length);

	MULTIUSER_PLUGIN_SYMBOL int UpdateNetworking();

#define STRLEN 128
	static char strBufIn[STRLEN];
	static char strBufOut[STRLEN];

	MULTIUSER_PLUGIN_SYMBOL char GetStrBufOut(int index);



#ifdef __cplusplus
}
#endif // __cplusplus

#endif // PLUGIN_H
