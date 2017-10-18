#include "LogWriter.h"

using namespace std;

void LogWriter::writeToLog(std::string message)
{
	ofstream file;
	file.open(directory+fileName, ofstream::app);	//Try to open the file and append to it

	file << message + "\n";	//Add the new line of text to the file with time stamp TODO:Add time stamp
	file.close();
}

void LogWriter::resetLog()
{
	ofstream file;
	file.open(directory + fileName);	//Should clear the file
	//TODO: Add intial timestamp for starting and other relevent info about the plugin
	file.close();
}
