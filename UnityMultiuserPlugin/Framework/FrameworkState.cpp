#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum)
{
	writeToLogger("In Framework StartServer Function");

	std::string temp = std::to_string(portNum);
	RakNet::SocketDescriptor sd(portNum, 0);	//Calls to start server
	writeToLogger("Made Socket Descriptor on " + temp);
	mpPeer->Startup(maxClients, &sd, 1);
	writeToLogger("Started Peer");
	mpPeer->SetMaximumIncomingConnections(maxClients);
	temp = std::to_string(maxClients);
	writeToLogger("Set Max connections to " + temp);
	return true;
}

bool FrameworkState::StartClient(char * targetIP, int portNum)
{
	writeToLogger("In Framework StartClient Function");

	std::string temp = std::to_string(portNum);

	RakNet::SocketDescriptor sd;	//Calls to properly connect to server
	mpPeer->Startup(1, &sd, 1);
	writeToLogger("Made Socket Descriptor on " + temp);

	mpPeer->Connect(targetIP, portNum, 0, 0);
	temp = targetIP;
	writeToLogger("Connecting to server on ip " + temp); //TODO: Properly send char array to this function

	mTargetIP = targetIP; 
	return true;
}

char* FrameworkState::UpdateNetwork()
{

	RakNet::Packet *packet;

	packet = mpPeer->Receive();
	if (packet)
	{
		writeToLogger("There is data");
		writeToLogger((char*)packet->data);
		return (char*)packet->data;
	}
	else
	{
		//writeToLogger("No Data");
		return NULL;	
	}
}

void FrameworkState::resetLogger()
{
	pLogger.resetLog();
}

void FrameworkState::writeToLogger(std::string message)
{
	pLogger.writeToLog(message);
}


bool FrameworkState::SendData(char* data, int length)
{
	writeToLogger(data);
	RakNet::SystemAddress newAddress = RakNet::SystemAddress(mTargetIP.c_str());	//Convert passed string into raknet IP address
	mpPeer->Send(data, sizeof(data), HIGH_PRIORITY, RELIABLE_ORDERED, 0, newAddress, false);
	return true;
}

bool FrameworkState::BroadCastData(char data[], int length, char ip[])
{
	//TODO: Relay data to all connected client EXCEPT the client with the given ip, prevents ghosting

	return false;
}
