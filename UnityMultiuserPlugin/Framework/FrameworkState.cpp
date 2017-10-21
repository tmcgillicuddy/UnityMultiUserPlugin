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

	return true;
}

int FrameworkState::UpdateNetwork()
{
	//writeToLogger("Updating Network");
	for (mpPacket = mpPeer->Receive(); mpPacket; mpPeer->DeallocatePacket(mpPacket), mpPacket = mpPeer->Receive())
	{
		switch (mpPacket->data[0])
		{
		case ID_REMOTE_DISCONNECTION_NOTIFICATION:
			writeToLogger("Another client has disconnected");
			break;
		case ID_REMOTE_CONNECTION_LOST:
			writeToLogger("Another client has lost connection");
			break;
		case ID_REMOTE_NEW_INCOMING_CONNECTION:
			writeToLogger("Another client has connected");
			break;
		case ID_CONNECTION_REQUEST_ACCEPTED:
			writeToLogger("Connection is Accepted");
			break;
		case ID_NEW_INCOMING_CONNECTION:
			writeToLogger("New Client Is Connecting");
			//TODO: Send all the data from the server to the client
			//TODO: Add the connected client's info to the server list of users, 
			//TODO: Inform all connected clients that someone else has entered
			//TODO: Send assingment data BACK to the newly connected Client

			break;
		case ID_NO_FREE_INCOMING_CONNECTIONS:
			writeToLogger("Connection Failed, Server is FULL");
			break;
		case ID_DISCONNECTION_NOTIFICATION:
			if (isServer) {
				writeToLogger("A client has disconnected");
			}
			else {
				writeToLogger("We have been disconnected");
			}
			break;
		case ID_CONNECTION_ATTEMPT_FAILED:
		{
			writeToLogger("Failed to connect to server");
		}
		break;
		case ID_CONNECTION_LOST:
			if (isServer) {
				writeToLogger("A client has lost connection");
			}
			else {
				writeToLogger("Connection lost");
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
