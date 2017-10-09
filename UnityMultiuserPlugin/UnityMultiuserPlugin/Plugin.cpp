#include "Plugin.h"

// STUFF TO BE WRAPPED
#include "../Framework/FrameworkState.h"

// RakNet includes
#include "RakNet/RakPeerInterface.h"
#include "RakNet/RakNetTypes.h"
#include "RakNet/MessageIdentifiers.h"

using namespace RakNet;

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

int StartServer(/*params*/)
{

	return 0;
}

int StartClient(/*params*/)
{

	return 0;
}

int SendData(/*params*/)
{

	return 0;
}