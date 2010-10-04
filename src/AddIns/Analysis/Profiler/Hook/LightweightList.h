// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
