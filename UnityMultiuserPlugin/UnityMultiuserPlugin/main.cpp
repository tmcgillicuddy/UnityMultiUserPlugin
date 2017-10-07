#include "Plugin.h"

#include <iostream>
using namespace std;

int main()
{
	Startup();

	cout << Foo(2) << endl;

	Shutdown();
}