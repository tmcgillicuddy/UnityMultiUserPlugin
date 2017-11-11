#include "FrameworkState.h"

void FrameworkState::resetLogger()
{
	pLogger.resetLog();
}

void FrameworkState::writeToLogger(std::string message)
{
	pLogger.writeToLog(message);
}