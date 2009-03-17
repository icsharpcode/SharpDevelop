// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "global.h"
#include "FunctionInfo.h"
#include <cassert>

const int defaultTableSize = 4;

void FunctionInfo::Check()
{
	#ifdef DEBUG
	int fc = 0;
	for (int i = 0; i <= LastChildIndex; i++) {
		if (Children[i] != nullptr) {
			fc++;
		}
	}
	assert(fc == FillCount);
	#endif
}

void FunctionInfo::AddOrUpdateChild(FunctionInfo* child)
{
	int slot = child->Id;
	for (;;) {
		slot &= this->LastChildIndex;
		FunctionInfo *slotContent = this->Children[slot];
		if (slotContent == nullptr || slotContent->Id == child->Id) {
			this->Children[slot] = child;
			break;
		}
		slot++;
	}
}

FunctionInfo* CreateFunctionInfo(int id, int indexInParent)
{
	// Allocate the child in memory
	FunctionInfo* newFunction = (FunctionInfo *)sharedMemoryHeader->mallocator.malloc(sizeof(FunctionInfo) + defaultTableSize * sizeof(void*));
	// the allocater takes care of zeroing the memory
	
	// Set field values
	newFunction->Id = id;
	newFunction->TimeSpent = (ULONGLONG)indexInParent << 56;
	// Initialize the table
	newFunction->LastChildIndex = defaultTableSize - 1;
	// Return pointer to the created child
	return newFunction;
}

FunctionInfo* FunctionInfo::Resize(int newTableSize)
{
//	DebugWriteLine("Resize %d", Id);
	// Allocate space for the copy
	FunctionInfo* newFunction = (FunctionInfo *)sharedMemoryHeader->mallocator.malloc(sizeof(FunctionInfo) + newTableSize * sizeof(void*));

	// Copy the header
	memcpy(newFunction, this, sizeof(FunctionInfo));
	// Initialize the new table
	newFunction->LastChildIndex = newTableSize - 1;
	// Copy the old table entries
	int oldTableSize = this->LastChildIndex + 1;
	for(int i = 0; i < oldTableSize; i++) {
		FunctionInfo *child = this->Children[i];
		if (child != nullptr) {
			newFunction->AddOrUpdateChild(child);
		}
	}
	// we cannot delete the original yet - we need to wait until all pointers to it have been updated,
	// because updating a pointer is safely possible only while the original still exists!
	// Done - FunctionInfo was resized
	return newFunction;
}

FunctionInfo* FunctionInfo::GetOrAddChild(int functionID, FunctionInfo*& newParent)
{
	int lastChildIndex = this->LastChildIndex;
	int slot = functionID;
	for (;;) {
		slot &= lastChildIndex;
		FunctionInfo *slotContent = this->Children[slot];
		if (slotContent == nullptr) {
			this->FillCount++;
			if (this->FillCount * 4 >= lastChildIndex * 3) {
				// resize table
				newParent = this->Resize(2 * (lastChildIndex + 1));
				slotContent = CreateFunctionInfo(functionID, newParent->FillCount);
				newParent->AddOrUpdateChild(slotContent);
				newParent->Check();
			} else {
				slotContent = CreateFunctionInfo(functionID, this->FillCount);
				this->Children[slot] = slotContent;
			}
			return slotContent;
		} else if (slotContent->Id == functionID) {
			return slotContent;
		}
		slot++;
	}
}

void FreeFunctionInfo(FunctionInfo* f)
{
	sharedMemoryHeader->mallocator.free(f, sizeof(FunctionInfo) + (f->LastChildIndex + 1) * sizeof(FunctionInfo*));
}
