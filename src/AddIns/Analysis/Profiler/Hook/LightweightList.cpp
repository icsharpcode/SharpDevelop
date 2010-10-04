// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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