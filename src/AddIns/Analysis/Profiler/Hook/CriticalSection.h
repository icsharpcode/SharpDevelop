// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
