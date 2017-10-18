#ifndef _LIB_H
#define _LIB_H

// tags for exporting or importing function
#ifdef MULTIUSER_PLUGIN_EXPORT
#define MULTIUSER_PLUGIN_SYMBOL __declspec(dllexport)
#else	// !MULTIUSER_PLUGIN_EXPORT
#ifdef MULTIUSER_PLUGIN_IMPORT
#define MULTIUSER_PLUGIN_SYMBOL __declspec(dllimport)
#else // !MULTIUSER_PLUGIN_IMPORT
#define MULTIUSER_PLUGIN_SYMBOL
#endif // MULTIUSER_PLUGIN_IMPORT
#endif //MULTIUSER_PLUGIN_EXPORT


#endif // _LIB_H
