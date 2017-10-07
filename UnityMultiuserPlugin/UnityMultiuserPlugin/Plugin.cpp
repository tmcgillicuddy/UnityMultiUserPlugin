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