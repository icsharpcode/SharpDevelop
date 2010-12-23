// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma once

#include "main.h"
#include "allocator.h"
#include "sharedMemory.h"
#include "Profiler.h"
#include "LightweightStack.h"
#include "LightweightList.h"

#define NAME_BUFFER_SIZE 1024

#define ASSEMBLY_INJECTION_NAME_LIST_LENGTH 3
#define CONSOLE_GROUP_LENGTH 2
#define WINFORMS_GROUP_LENGTH 4
#define WPF_GROUP_LENGTH 1

extern fastAllocator stackAllocator;
extern SharedMemoryHeader *sharedMemoryHeader;
extern CProfiler profiler;
extern HANDLE listMutex;
extern LightweightList *allThreadLocalDatas;
extern DWORD tls_index;

extern WCHAR *assemblyInjectionNameList[ASSEMBLY_INJECTION_NAME_LIST_LENGTH];
extern WCHAR *consoleGroupList[CONSOLE_GROUP_LENGTH];
extern WCHAR *winFormsGroupList[WINFORMS_GROUP_LENGTH];
extern WCHAR *wpfGroupList[WPF_GROUP_LENGTH];

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
	volatile bool active;
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

class HandlerInfo {
	public:
		mdMethodDef deactivator;
		mdMethodDef activator;
		mdMethodDef logger;
		
		HandlerInfo(mdMethodDef activator, mdMethodDef deactivator, mdMethodDef logger)
		{
			this->activator = activator;
			this->deactivator = deactivator;
			this->logger = logger;
		}
};

const COR_SIGNATURE rgSig[] = { IMAGE_CEE_CS_CALLCONV_DEFAULT, 0, ELEMENT_TYPE_VOID };

const COR_SIGNATURE loggerSig[] = { IMAGE_CEE_CS_CALLCONV_DEFAULT, 4, ELEMENT_TYPE_VOID, ELEMENT_TYPE_I4, ELEMENT_TYPE_I4, ELEMENT_TYPE_STRING, ELEMENT_TYPE_STRING };

const COR_SIGNATURE getNameSig[] = { IMAGE_CEE_CS_CALLCONV_HASTHIS, 0, ELEMENT_TYPE_STRING };