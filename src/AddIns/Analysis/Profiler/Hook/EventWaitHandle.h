// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma once

#include "windows.h"

class CEventWaitHandle
{
public:
	CEventWaitHandle(char *name);
	bool Set();
	bool Reset();
	DWORD Wait();
	DWORD Wait(int milliseconds); 
	HANDLE GetEventHandle();
private:
	char *name;
	HANDLE eventHandle;
};

CEventWaitHandle *OpenEventWaitHandle(char *name);
char *GetEventName(int eventName);




