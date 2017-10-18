#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum)
{
	writeToLogger("In Framework StartServer Function");
	RakNet::SocketDescriptor sd(portNum, 0);	//Calls to start server
	writeToLogger("Made Socket Descriptor on "+ portNum);
	mpPeer->Startup(maxClients, &sd, 1);
	writeToLogger("Started Peer");
	mpPeer->SetMaximumIncomingConnections(maxClients);
	writeToLogger("Set Max connections to " + maxClients);
	return true;
}

bool FrameworkState::StartClient(char targetIP[], int portNum)
{
	RakNet::SocketDescriptor sd;	//Calls to properly connect to server
	mpPeer->Startup(1, &sd, 1);
	writeToLogger("Made Socket Descriptor on " + portNum);
	mpPeer->Connect(targetIP, portNum, 0, 0);
	writeToLogger("Connecting to server on ip "); //TODO: add targetIP

	return true;
}

int FrameworkState::UpdateNetwork()
{
	for (mpPacket = mpPeer->Receive(); mpPacket; mpPeer->DeallocatePacket(mpPacket), mpPacket = mpPeer->Receive())
	{
		switch (mpPacket->data[0])
		{
		case ID_REMOTE_DISCONNECTION_NOTIFICATION:
			printf("Another client has disconnected.\n");
			break;
		case ID_REMOTE_CONNECTION_LOST:
			printf("Another client has lost the connection.\n");
			break;
		case ID_REMOTE_NEW_INCOMING_CONNECTION:
			printf("Another client has connected.\n");
			break;
		case ID_CONNECTION_REQUEST_ACCEPTED:
			printf("Our connection request has been accepted.\n");
			break;
		case ID_NEW_INCOMING_CONNECTION:
			return true;
			printf("A connection is incoming.\n");
			break;
		case ID_NO_FREE_INCOMING_CONNECTIONS:
			printf("The server is full.\n");
			break;
		case ID_DISCONNECTION_NOTIFICATION:
			if (isServer) {
				printf("A client has disconnected.\n");
			}
			else {
				printf("We have been disconnected.\n");
			}
			break;
		case ID_CONNECTION_LOST:
			if (isServer) {
				printf("A client lost the connection.\n");
			}
			else {
				printf("Connection lost.\n");
			}
			break;
		default:
			printf("Message with identifier %i has arrived.\n", mpPacket->data[0]);
			break;
		}
	}
	return false;
}

void FrameworkState::resetLogger()
{
	pLogger.resetLog();
}

void FrameworkState::writeToLogger(std::string message)
{
	pLogger.writeToLog(message);
}

bool FrameworkState::SendData(char data[], int length)
{
	//TODO: Either send the data to all connected clients or to the server

	return false;
}

bool FrameworkState::BroadCastData(char data[], int length, char ip[])
{
	//TODO: Relay data to all connected client EXCEPT the client with the given ip, prevents ghosting

	return false;
}
