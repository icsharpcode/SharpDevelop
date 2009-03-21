// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
