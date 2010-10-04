// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma once
#include "main.h"

class CriticalSection {
	CRITICAL_SECTION criticalSection;
	
	// don't support copy constructor for critical sections
	CriticalSection(const CriticalSection&);
	CriticalSection& operator=(const CriticalSection&);
	
public:
	CriticalSection() {
		if (!InitializeCriticalSectionAndSpinCount(&criticalSection, 0x80000400))
			throw "Couldn't initialize critical section";
	}
	~CriticalSection() {
		DeleteCriticalSection(&criticalSection);
	}
	void Enter() {
		EnterCriticalSection(&criticalSection);
	}
	void Leave() {
		LeaveCriticalSection(&criticalSection);
	}
};
