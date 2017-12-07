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
#include "LogWriter.h"

using namespace std;

void LogWriter::writeToLog(std::string message)
{
	ofstream file;
	file.open(directory+fileName, ofstream::app);	//Try to open the file and append to it

	file << message + "\n";	//Add the new line of text to the file with time stamp 
	file.close();
}

void LogWriter::drawLine()
{
	ofstream file;
	file.open(directory + fileName, ofstream::app);	//Try to open the file and append to it

	file << "----------------------------------------------------------------------\n";	//Add the new line of text to the file with time stamp 
	file.close();
}

void LogWriter::resetLog()
{
	ofstream file;
	file.open(directory + fileName);	//Should clear the file
	file.close();
}
