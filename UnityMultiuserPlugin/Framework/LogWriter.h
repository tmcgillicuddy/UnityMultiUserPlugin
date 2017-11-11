#pragma once

#include <iostream>
#include <fstream>
#include <string>

class LogWriter
{
public:
	void writeToLog(std::string message);	//Appends to the current Log file
	void drawLine();
	void resetLog();	//Clears the current log file

private:

	std::string fileName = "MultiuerPluggingLog.txt";	//Name of the text file where things will get logged
	std::string directory = "";	//Default to local

};
