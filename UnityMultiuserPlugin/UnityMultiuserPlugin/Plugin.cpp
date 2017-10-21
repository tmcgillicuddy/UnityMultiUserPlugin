#include "Plugin.h"

// STUFF TO BE WRAPPED
#include "../Framework/FrameworkState.h"

MULTIUSER_PLUGIN_SYMBOL FrameworkState *theState = 0;


int Startup()
{
	if (theState == 0)
	{
		theState = new FrameworkState;
		theState->resetLogger();
		theState->writeToLogger("Startup Complete");
		return 1;
	}
	return 0;
}

int Shutdown()
{
	if (theState != 0)
	{
		theState->writeToLogger("Shutting Down");
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
		theState->writeToLogger("Running Test FOO function " + bar);
		return theState->StateFoo(bar);
	}
	return 0;
}

int StartServer(int maxClients, int portNum)
{
	if (theState != 0)
	{
		theState->writeToLogger("Starting a Server");
		return theState->StartServer(maxClients, portNum);
	}
	return 0;
}

int StartClient(char targetIP[], int portNum)
{
	if (theState != 0)
	{
		theState->writeToLogger("Starting a Client");
		return 	theState->StartClient(targetIP, portNum);
	}
	return 0;
}

int SendData(char data[], int length)
{
	if (theState != 0)
	{
		theState->writeToLogger("Sending Data");
		return theState->SendData(data, length);
	}
	return 0;
}


int UpdateNetworking()
{
	if (theState != 0)
	{
		theState->writeToLogger("Updating Network");
		return theState->UpdateNetwork();
	}
	return 0;
}

MULTIUSER_PLUGIN_SYMBOL char GetStrBufOut(int index)
{
	if (theState != 0)
	{
		return strBufOut[index];
	}
}
