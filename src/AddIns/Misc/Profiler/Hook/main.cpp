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

extern "C" {
	void __stdcall DeactivateProfiler()
	{
		profiler.LogString(L"DeactivateProfiler called!");
		profiler.Deactivate();
	}
	
	void __stdcall ActivateProfiler()
	{
		profiler.LogString(L"ActivateProfiler called!");
		profiler.Activate();
	}
	
	void __stdcall LogEvent(int type, int id, WCHAR *controlName, WCHAR *controlText)
	{
		WCHAR *format = L"";
	
		switch (type) {
			case 0:
				format = L"event %d %d --";
				break;
			case 1:
				format = L"event %d %d --";
				break;
			case 2:
				format = L"event %d %d -name:\"%s\"text:\"%s\"-";
				break;
			case 3:
				break;
		}
	
		DebugWriteLine(format, type, id, controlName, controlText);
	}
}