#include "FrameworkState.h"

int FrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

bool FrameworkState::StartServer(int maxClients, int portNum)
{
	std::string temp = std::to_string(portNum);
	writeToLogger("In Framework StartServer Function");
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
	std::string temp = std::to_string(portNum);

	RakNet::SocketDescriptor sd;	//Calls to properly connect to server
	mpPeer->Startup(1, &sd, 1);
	writeToLogger("Made Socket Descriptor on " + temp);

	mpPeer->Connect(targetIP, portNum, 0, 0);
	temp = targetIP;
	writeToLogger("Connecting to server on ip " + temp); //TODO: Properly send char array to this function

	return true;
}

int FrameworkState::UpdateNetwork()
{
	//writeToLogger("Updating Network");
	for (mpPacket = mpPeer->Receive(); mpPacket; mpPeer->DeallocatePacket(mpPacket), mpPacket = mpPeer->Receive())
	{
		switch (mpPacket->data[0])	//TODO: Replace all printfs with write to logger calls
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
			writeToLogger("Connection is Accepted");
			printf("Our connection request has been accepted.\n");
			break;
		case ID_NEW_INCOMING_CONNECTION:
			writeToLogger("New Client Is Connecting");
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
		case ID_CONNECTION_ATTEMPT_FAILED:
		{
			writeToLogger("Failed to connect to server");
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
			writeToLogger("Message with identifier "+ std::to_string(mpPacket->data[0]) +" has arrived");
			break;
		}
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
