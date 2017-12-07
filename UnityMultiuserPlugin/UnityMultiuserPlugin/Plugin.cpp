/*
EGP 405-01 Final Project 12/7/17
Aaron Hamilton
James Smith
Thomas McGillicuddy
“We certify that this work is
entirely our own. The assessor of this project may reproduce this project
and provide copies to other academic staff, and/or communicate a copy of
this project to a plagiarism-checking service, which may retain a copy of the
project on its database.”
*/
#include "Plugin.h"
#include "../Framework/ServerState.h"
#include "../Framework/ClientState.h"

// STUFF TO BE WRAPPED
#include "../Framework/FrameworkState.h"

MULTIUSER_PLUGIN_SYMBOL FrameworkState *theState = 0;

int Shutdown()
{
	if (theState != 0)
	{
		theState->writeToLogger("Shutting Down");
		theState->cleanup();
		delete theState;
		theState = 0;

		return 1;
	}
	return 0;
}

int StartServer(char* password, int portNum, int maxClients)
{
	if (theState == 0)
	{
		theState = new ServerState;
		theState->resetLogger();
		theState->drawLineOnLogger();
		theState->writeToLogger("Starting a Server");
		bool temp = theState->init("", portNum, maxClients);
		theState->drawLineOnLogger();
		return temp;
	}
	return 0;
}

int StartClient(char *targetIP, int portNum)
{
	if (theState == 0)
	{
		theState = new ClientState;
		theState->resetLogger();
		theState->drawLineOnLogger();
		theState->writeToLogger("Starting a Client");
		theState->writeToLogger(targetIP);
		bool temp = theState->init(targetIP, portNum, 0);
		theState->drawLineOnLogger();
		return temp;
	}
	return 0;
}

int SendData(int mID, char* data, int length, char* ownerIP)
{
	if (theState != 0)
	{
		theState->writeToLogger("Sending Data");
		int good = theState->SendData(mID, data, length, ownerIP);
		theState->drawLineOnLogger();
		return good;
	}
	return 0;
}

char* GetData()
{
	if (theState != 0)
	{
		char *data = theState->UpdateNetwork();
		return data;
	}
	return 0;
}

char * GetLastPacketIP()
{
	if (theState != 0)
	{
		theState->writeToLogger("Getting last packet IP");
		char *data = theState->GetLastPacketIP();
		theState->writeToLogger(data);
		theState->drawLineOnLogger();
		return data;
	}

	return 0;
}
