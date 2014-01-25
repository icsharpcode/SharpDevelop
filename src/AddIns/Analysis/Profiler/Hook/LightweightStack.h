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

#include <cassert>

// A custom stack class with known memory layout - used across processes in the shared memory.
// Dynamic arrays are allocated in the shared memory using 'stackAllocator'.
// Not thread-safe, requires external synchronization.
template<typename T>
struct LightweightStack {
	T *array;
	T *topPointer;
	T *arrayEnd;
	
	// don't support copy constructor
	LightweightStack(const LightweightStack&);
	LightweightStack& operator=(const LightweightStack&);
	
	// don't inline Enlarge into push (Enlarge is rarely called)
	__declspec(noinline) void Enlarge() {
		ptrdiff_t capacity = (arrayEnd - array);
		ptrdiff_t newCapacity = 2 * capacity;
		ptrdiff_t topIndex = topPointer - array;
		T *newArray = (T *)stackAllocator.malloc(newCapacity * sizeof(T));
		memcpy(newArray, array, capacity * sizeof(T));
		stackAllocator.free(array, capacity * sizeof(T));
		array = newArray;
		arrayEnd = newArray + newCapacity;
		topPointer = newArray + topIndex;
	}
	
	LightweightStack() {
		array = (T *)stackAllocator.malloc(64 * sizeof(T));
		arrayEnd = array + 64;
		topPointer = array - 1;
	}
	
	~LightweightStack() {
		stackAllocator.free(array, (arrayEnd - array) * sizeof(T));
	}
	
	bool empty() {
		return topPointer < array;
	}
	
	bool hasAtLeastTwoElements() {
		return topPointer > array;
	}
	
	size_t size() {
		return (topPointer - array) + 1;
	}
	
	T &top() {
		assert(!empty());
		return *topPointer;
	}
	
	T &belowTop() {
		assert(size() >= 2);
		return *(topPointer - 1);
	}
	
	void push(const T& value) {
		++topPointer;
		if (topPointer == arrayEnd)
			Enlarge();
		assert(topPointer < arrayEnd);
		*topPointer = value;
	}
	
	void pop() {
		assert(!empty());
		topPointer--;
	}
};
