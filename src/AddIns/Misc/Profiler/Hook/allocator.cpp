// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "windows.h"
#include "allocator.h"
#include <cassert>

static inline bool isAligned(void* ptr)
{
	// test if ptr is multiple of sizeof(UINT_PTR):
	return ((UINT_PTR)ptr % sizeof(UINT_PTR)) == 0;
}

fastAllocator::fastAllocator()
	: startPos(nullptr), pos(nullptr), endPos(nullptr)
{
}

void fastAllocator::initialize(void* pos, void* endPos)
{
	assert(isAligned(pos));
	assert(pos != nullptr);
	assert(endPos > pos);
	
	assert(startPos == nullptr);
	
	this->startPos = pos;
	this->pos = pos;
	this->endPos = endPos;
}

void* fastAllocator::malloc(size_t bytes)
{
	assert(pos != nullptr); // ensure allocator is initialized before malloc is called
	
	// must be compatible with C# malloc! (used in Data Collection)
	
	#if DEBUG
	// space for debugging info
	const size_t debuggingInfoSize = 8; //sizeof(size_t); - always use 8 so we keep proper alignment for LONGLONGs
	bytes += debuggingInfoSize; 
	#endif
	
	#ifdef _M_AMD64
	void *t = (void*)_InterlockedExchangeAdd64((__int64 volatile*)&pos, (__int64)bytes);
	#else
	void *t = (void*)_InterlockedExchangeAdd((LONG volatile*)&pos, (LONG)bytes);
	#endif
	
	if ((byte*)endPos - (byte*)t > (ptrdiff_t)bytes) {
		#if DEBUG
		t = (byte*)t + debuggingInfoSize; // skip over debugging info
		((size_t*)t)[-1] = bytes - debuggingInfoSize;
		#endif
		assert(isAligned(t));
		return t;
	} else {
		return nullptr;
	}
}

#pragma warning(disable : 4100)
void fastAllocator::free(void* memory, size_t bytes)
{
	assert(memory != nullptr);
	assert(memory >= startPos && memory < endPos);
	#if DEBUG
	assert(((size_t*)memory)[-1] == bytes);
	#endif
}
#pragma warning(default : 4100)

template<class T> void freeListAllocator<T>::initialize(void* pos, void* endPos)
{
	allocator.initialize(pos, endPos);
}

template<class T> freeListAllocator<T>::freeListAllocator()
{
	for (int i = 0; i < freeListSize; i++) {
		freeList[i] = nullptr;
	}
	// check if the allocMappingFunc works for our allocation size
	const int numberOfAllocationSizes = sizeof(T::PossibleAllocationSizes) / sizeof(size_t);
	for (int i = 0; i < numberOfAllocationSizes; i++) {
		void* volatile & slot = freeList[T::allocMappingFunc( T::PossibleAllocationSizes[i] )];
		if (slot != nullptr) {
			std::string msg = "freeListAllocator<";
			msg += typeid(T).name();
			msg += "> Self Test Failed";
			MessageBox(nullptr, msg.c_str(), TEXT("fatal error!"), MB_OK);
			break;
		}
		slot = (void*)1;
	}
	
	for (int i = 0; i < freeListSize; i++) {
		freeList[i] = nullptr;
	}
}

template<class T> bool freeListAllocator<T>::isPossibleAllocationSize(size_t bytes)
{
	const int numberOfAllocationSizes = sizeof(T::PossibleAllocationSizes) / sizeof(size_t);
	for (int i = 0; i < numberOfAllocationSizes; i++) {
		if (T::PossibleAllocationSizes[i] == bytes)
			return true;
	}
	DebugWriteLine(L"Allocation of %Id bytes not possible.", bytes);
	return false;
}

template<class T> void* freeListAllocator<T>::malloc(size_t bytes)
{
	assert(bytes >= sizeof(void*));
	assert(isPossibleAllocationSize(bytes));
	void* volatile* freeListSlot = &freeList[T::allocMappingFunc(bytes)];
	void* t;
	void* next;
	do {
		t = *freeListSlot; // volatile read has acquire semantics
		if (t == nullptr)
			return allocator.malloc(bytes);
//		DebugWriteLine("Reusing memory of size %Id", bytes);
		next = *(void**)t;
	} while (InterlockedCompareExchangePointer(freeListSlot, next, t) != t);
	memset(t, 0, bytes);
	return t;
}
template<class T> void freeListAllocator<T>::free(void* memory, size_t bytes)
{
	// the base allocator cannot free, just allow it to check whether 'bytes' is the correct size
	allocator.free(memory, bytes);
	
	assert(bytes >= sizeof(void*));
	assert(isPossibleAllocationSize(bytes));
	
//	DebugWriteLine("Freeing memory of size %Id", bytes);
	void* volatile* freeListSlot = &freeList[T::allocMappingFunc(bytes)];
	void* t;
	do {
		t = *freeListSlot;
		*(void**)memory = t;
	} while (InterlockedCompareExchangePointer(freeListSlot, memory, t) != t);
}

// allocation sizes for AllocationSize types:

const size_t FunctionInfoAllocationSize::PossibleAllocationSizes[26] 
    = {32, 40, 56, 88, 152, 280, 536, 1048, 2072, 4120, 8216, 16408, 32792,
       65560, 131096, 262168, 524312, 1048600, 2097176, 4194328, 8388632,
       16777240, 33554456, 67108888, 134217752, 268435480};

// because the method implementations are not in the header file,
// we need to explicitly instantiate the freeListAllocator templates:
template struct freeListAllocator<FunctionInfoAllocationSize>;

