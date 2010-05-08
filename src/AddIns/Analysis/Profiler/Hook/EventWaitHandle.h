// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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




