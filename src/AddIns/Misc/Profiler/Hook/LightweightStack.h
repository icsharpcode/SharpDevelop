// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
