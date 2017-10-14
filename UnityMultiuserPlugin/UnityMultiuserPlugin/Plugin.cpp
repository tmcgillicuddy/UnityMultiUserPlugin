#include "Plugin.h"

// STUFF TO BE WRAPPED
#include "../Framework/FrameworkState.h"



MULTIUSER_PLUGIN_SYMBOL FrameworkState *theState = 0;

int Startup()
{
	if (theState == 0)
	{
		theState = new FrameworkState;
		return 1;
	}
	return 0;
}

int Shutdown()
{
	if (theState != 0)
	{
		delete theState;
		theState = 0;
		return 1;
	}
	return 0;
}

int Foo(int bar)
{
	if (theState != 0)
	{
		//return theState->StateFoo(bar);
	}
	return 0;
}

MULTIUSER_PLUGIN_SYMBOL int StartServer(int maxClients, int portNum, char password[])
{
	if (theState != 0)
	{
		//return 	theState->StartServer(maxClients, portNum, password);
	}
	return 0;
}

MULTIUSER_PLUGIN_SYMBOL int StartClient(char targetIP[], int portNum)
{
	if (theState != 0)
	{
		//return 	theState->StartClient(targetIP, portNum);
	}
	return 0;
}

MULTIUSER_PLUGIN_SYMBOL int SendData(char data[], int length)
{
	if (theState != 0)
	{
		//return 	theState->SendData(data, length);
	}
	return 0;
}
