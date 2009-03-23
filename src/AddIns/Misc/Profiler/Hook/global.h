// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#pragma once
#include "main.h"
#include "allocator.h"
#include "sharedMemory.h"
#include "Profiler.h"
#include "LightweightStack.h"
#include "LightweightList.h"

extern fastAllocator stackAllocator;
extern SharedMemoryHeader *sharedMemoryHeader;
extern CProfiler profiler;
extern HANDLE listMutex;
extern LightweightList *allThreadLocalDatas;
extern DWORD tls_index;

struct StackEntry {
	FunctionInfo *function;
	ULONGLONG startTime;
	int frameCount;
	
	StackEntry() {}
	StackEntry(FunctionInfo * function, ULONGLONG startTime) : function(function), startTime(startTime), frameCount(1) {}
};

struct ThreadLocalData {
	int threadID;
	volatile int inLock;
	ThreadLocalData *predecessor;
	ThreadLocalData *follower;

	LightweightStack<StackEntry> stack;

	ThreadLocalData();
};

ThreadLocalData *AttachToThread();

inline ThreadLocalData *getThreadLocalData() {
	return (ThreadLocalData*)TlsGetValue(tls_index);
}

class ThreadInfo {
	public:
		DWORD unmanagedThreadId;
		ThreadID managedThreadId;
		std::wstring threadName;
		int functionInfoId;
		
		ThreadInfo()
		{
			this->unmanagedThreadId = 0;
			this->managedThreadId = nullptr;
		}
};