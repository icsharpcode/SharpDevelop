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
#include "main.h"
#include "global.h"
#include "LightweightList.h"

LightweightList::LightweightList(HANDLE mutex)
{
	this->lastItem = nullptr;
	this->mutex = mutex;
}

ThreadLocalData *LightweightList::add()
{
	WaitForSingleObject(this->mutex, INFINITE);

	void *itemMemory = stackAllocator.malloc(sizeof(ThreadLocalData));
	assert(itemMemory != nullptr);
	ThreadLocalData *item = new (itemMemory) ThreadLocalData();
	
	item->active = true;

	if (this->lastItem == nullptr) {
		this->lastItem = item;
		this->lastItem->follower = nullptr;
		this->lastItem->predecessor = nullptr;
	} else {
		this->lastItem->follower = item;
		item->follower = nullptr;
		item->predecessor = this->lastItem;
		this->lastItem = item;
	}

	sharedMemoryHeader->LastThreadListItem = this->lastItem;

	ReleaseMutex(this->mutex);

	return item;
}

void LightweightList::remove(ThreadLocalData *item)
{
	WaitForSingleObject(this->mutex, INFINITE);

	assert(item != nullptr);
	assert(this->lastItem != nullptr);

	if (item == this->lastItem) {
		this->lastItem = this->lastItem->predecessor;
		if (this->lastItem != nullptr)
			this->lastItem->follower = nullptr;
	} else {
		item->follower->predecessor = item->predecessor;
		if (item->predecessor != nullptr)
			item->predecessor->follower = item->follower;
	}

	item->~ThreadLocalData();

	stackAllocator.free(item, sizeof(ThreadLocalData));

	sharedMemoryHeader->LastThreadListItem = this->lastItem;

	ReleaseMutex(this->mutex);
}