// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

#pragma once

// A thread-safe allocator that assigns memory from a contiguous memory block. Memory cannot be freed. 
struct fastAllocator {
	void* startPos;
	void* volatile pos;
	void* endPos;
public:
	fastAllocator();
	void initialize(void* pos, void* endPos);
	
	void* malloc(size_t bytes);
	
	void free(void* memory, size_t bytes);
};

// A thread-safe allocator that internally uses a fastAllocator, but allows re-using freed memory blocks
// The possible memory allocations must be allowed by the AllocationSizes class provided,
// and must be at least sizeof(void*) bytes.
template<typename AllocationSizes> struct freeListAllocator {
	fastAllocator allocator;
	static const int freeListSize = AllocationSizes::FreeListSize;
	void* volatile freeList[freeListSize];
	
	bool isPossibleAllocationSize(size_t bytes);
public:
	freeListAllocator();

	void initialize(void* pos, void* endPos);

	void* malloc(size_t bytes);
	void free(void* memory, size_t bytes);
};

struct FunctionInfoAllocationSize {
public:
	static const int FreeListSize = 32;
	static const size_t PossibleAllocationSizes[26];
	
	static inline UINT_PTR allocMappingFunc(size_t bytes)
	{
		return (UINT_PTR)(((bytes * 374152163ULL) >> 32) & 31);
	}
};
