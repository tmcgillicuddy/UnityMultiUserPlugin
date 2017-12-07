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
#include "ServerState.h"

ServerState::ServerState()
	:FrameworkState()
{
}

ServerState::~ServerState()
{
	
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

bool ServerState::SendData(int mID, char * data, int length, char * ownerIP)
{
	writeToLogger(data);

	std::string tempDebug = ownerIP;

	writeToLogger("Sending data to " + tempDebug);

	dataBuffer* tempBuffer = new dataBuffer();
	tempBuffer->messageID = mID;
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

char * ServerState::GetLastPacketIP()
{
	std::string ip = lastPacket->systemAddress.ToString();
	char* tempIP = new char[ip.length()+1];
	//writeToLogger(lastPacket->systemAddress.ToString());
	if (lastPacket == nullptr)
	{
		writeToLogger("No Last Packet");
		return 0;
	}
	strcpy(tempIP,ip.c_str());

	return tempIP;
}

bool ServerState::cleanup()
{
	writeToLogger("Shutting down server");
	mpPeer->Shutdown(500, 0, LOW_PRIORITY);
	drawLineOnLogger();
	return true;
}

char * ServerState::UpdateNetwork()
{
	
	RakNet::Packet *packet;

	packet = mpPeer->Receive();
	//writeToLogger("Got Packet");
	if (packet)
	{
		lastPacket = packet;
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
