// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
