#include "ClientState.h"

ClientState::ClientState()
	:FrameworkState()
{
}

ClientState::~ClientState()
{
	mpPeer->Shutdown(500, 0, LOW_PRIORITY);
}

bool ClientState::init(char *targetIP, int portNum, int maxClients)
{
	writeToLogger("In Framework StartClient Function");

	std::string temp = std::to_string(portNum);

	RakNet::SocketDescriptor sd;	//Calls to properly connect to server
	mpPeer->Startup(1, &sd, 1);
	writeToLogger("Made Socket Descriptor on " + temp);

	mpPeer->Connect(targetIP, portNum, 0, 0);
	temp = targetIP;
	writeToLogger("Connecting to server on ip " + temp); //TODO: Properly send char array to this function

	mServerIP = targetIP;
	return true;
}

bool ClientState::SendData(char * data, int length, char * ownerIP)
{
	writeToLogger("Sending data to " + mServerIP);

	dataBuffer * tempBuffer = new dataBuffer();
	strcpy(tempBuffer->buffer, data);
	writeToLogger((char*)&tempBuffer);
	RakNet::SystemAddress newAddress = RakNet::SystemAddress(mServerIP.c_str());	//Convert passed string into raknet IP address
	if (mpPeer == NULL)
	{
		writeToLogger("Error with Peer");
		return false;
	}
	else
	{
		mpPeer->Send((char*)tempBuffer, sizeof(dataBuffer), HIGH_PRIORITY, RELIABLE_ORDERED, 0, newAddress, true);
		writeToLogger("Sent data");
		return true;
	}
}

char * ClientState::UpdateNetwork()
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
