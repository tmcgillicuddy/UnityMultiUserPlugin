#include "ServerState.h"

ServerState::ServerState()
	:FrameworkState()
{
}

ServerState::~ServerState()
{
	writeToLogger("Shutting down server");
	mpPeer->Shutdown(500, 0, LOW_PRIORITY);
	drawLineOnLogger();
}

bool ServerState::init(char *targetIP, int portNum, int maxClients)
{
	writeToLogger("In Framework StartServer Function");

	std::string temp = std::to_string(portNum);
	RakNet::SocketDescriptor sd(portNum, 0);	//Calls to start server
	writeToLogger("Made Socket Descriptor on " + temp);
	writeToLogger(std::to_string(maxClients));
	mpPeer->Startup(maxClients, &sd, 1);
	writeToLogger("Started Peer");
	mpPeer->SetMaximumIncomingConnections(maxClients);
	temp = std::to_string(maxClients);
	writeToLogger("Set Max connections to " + temp);
	return true;
	
}

bool ServerState::SendData(char * data, int length, char * ownerIP)
{
	writeToLogger(data);

	std::string tempDebug = ownerIP;

	writeToLogger("Sending data to " + tempDebug);

	dataBuffer* tempBuffer = new dataBuffer();
	strcpy(tempBuffer->buffer, data);
	RakNet::SystemAddress newAddress = RakNet::SystemAddress(ownerIP);	//Convert passed string into raknet IP address

	if (mpPeer == NULL)
	{
		writeToLogger("Error with Peer");
		return false;
	}
	else
	{
		mpPeer->Send((char*)tempBuffer, sizeof(dataBuffer), HIGH_PRIORITY, RELIABLE_ORDERED, 0, newAddress, false);
		writeToLogger("Sent data");
		return true;
	}
}

char * ServerState::UpdateNetwork()
{
	
	RakNet::Packet *packet;

	packet = mpPeer->Receive();
	//writeToLogger("Got Packet");
	if (packet)
	{
		writeToLogger("There is data");
		writeToLogger((char*)packet->data);
		drawLineOnLogger();
		return (char*)packet->data;
	}
	else
	{
		//writeToLogger("No Data");
		return NULL;
	}
}
