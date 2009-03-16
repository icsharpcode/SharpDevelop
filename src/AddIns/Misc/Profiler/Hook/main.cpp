// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "global.h"
#include "FunctionInfo.h"

//sharedMemoryAllocator mallocator;
freeListAllocator<FunctionInfoAllocationSize> mallocator;
fastAllocator stackAllocator;
CProfiler profiler;
FunctionInfo* currentFunction; // Thread-local variable
SharedMemoryHeader *sharedMemoryHeader;

#ifdef DEBUG
void DebugWriteLine(WCHAR *pszFmtString, ...)
{
	const int bufferSize = 2 * 4096;
	WCHAR szBuffer[bufferSize]; 
	
	DWORD dwWritten = swprintf_s(szBuffer, bufferSize, L"Profiler (thread %d): ", GetCurrentThreadId());
	
	va_list args;
	va_start( args, pszFmtString );
	vswprintf_s(&szBuffer[dwWritten], bufferSize - dwWritten, pszFmtString, args );
	va_end( args );
	
	OutputDebugStringW(szBuffer);
}
#endif

ThreadLocalData::ThreadLocalData()
{
	this->threadID = GetCurrentThreadId();
}

STDAPI rdtsc(ULONGLONG *tsc) {
 *tsc = __rdtsc();
 return S_OK;
}