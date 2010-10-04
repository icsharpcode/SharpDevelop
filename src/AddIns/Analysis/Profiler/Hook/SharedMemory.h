// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma once

#include "allocator.h"

struct FunctionInfo;
struct ThreadLocalData;

struct SharedMemoryHeader
{
	int Magic;  // Verfication value, always '~SM1'
	volatile int ExclusiveAccess;
	int TotalLength;
	int NativeToManagedBufferOffset;
	int ThreadDataOffset;
	int ThreadDataLength;
	int HeapOffset;
	int HeapLength;
	void* TargetPointer;
	FunctionInfo* RootFuncInfo;
	ThreadLocalData* LastThreadListItem;
	int ProcFrequency;
	bool doNotProfileDotnetInternals;
	bool combineRecursiveFunction;
	bool trackEvents;
	freeListAllocator<FunctionInfoAllocationSize> mallocator;
};

class CSharedMemory
{
public:
	CSharedMemory(TCHAR *name);
	~CSharedMemory();
	void* GetStartPtr();
	SharedMemoryHeader *header;
private:
	HANDLE fileHandle;
	void* startPtr;
	int length;
};
