// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#include "main.h"
#include "global.h"
#include "FunctionInfo.h"
#include "ProfilerMetaData.h"

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

	
std::wstring EscapeString(const WCHAR *input)
{
	std::wostringstream output;
	output.str(L"");
	
	for (const WCHAR *ptr = input; *ptr != 0; ptr++) {
		WCHAR c = *ptr;
		if (c == '"')
			output << "\"\"";
		else
			output << c;
	}
	
	return output.str();
}

extern "C" {
	void __stdcall DeactivateProfiler()
	{
		profiler.Deactivate();
	}
	
	void __stdcall ActivateProfiler()
	{
		profiler.Activate();
	}
	
	void __stdcall LogEvent(int type, int id, WCHAR *controlName, WCHAR *controlText)
	{
		if (controlName == nullptr)
			controlName = L"";
		if (controlText == nullptr)
			controlText = L"";
	
		profiler.LogString(L"event %d %d \"%s:%s\"", type, id, EscapeString(controlName).c_str(), EscapeString(controlText).c_str());
	}
}