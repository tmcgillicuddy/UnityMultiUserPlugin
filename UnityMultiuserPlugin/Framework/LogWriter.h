#pragma once
#define _ITERATOR_DEBUG_LEVEL 0	//For compiling in release mode properly

#include <iostream>
#include <fstream>
#include <string>

class LogWriter
{
public:
	void writeToLog(std::string message);	//Appends to the current Log file

	void resetLog();	//Clears the current log file

private:

	std::string fileName = "MultiuerPluggingLog.txt";	//Name of the text file where things will get logged
	std::string directory = "";	//Default to local

};
