// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Siegfried Pammer"/>
//     <version>$Revision$</version>
// </file>

#pragma once

#include <cassert>
#include "global.h"

// The LightweightList itself is not in shared memory,
// but its nodes are.
// Uses a Mutex for multithreaded access.
class LightweightList
{
public:
	LightweightList(HANDLE mutex);

	ThreadLocalData *add();
	void remove(ThreadLocalData *item);
private:
	ThreadLocalData *lastItem;
	HANDLE mutex;
};
