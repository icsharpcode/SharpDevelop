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

#include "main.h"
#include "EventWaitHandle.h"

/// Default constructor for CEventWaitHandle
/// Creates a new CEventWaitHandle with the specified name.
/// It can only open an existing Event.
/// Do NOT use DIRECTLY, use OpenEventWaitHandle instead!
CEventWaitHandle::CEventWaitHandle(char *name)
{
	this->name = name;
	this->eventHandle = OpenEventA(EVENT_MODIFY_STATE | SYNCHRONIZE, false, TEXT(this->name));
}

/// Sets the Event
bool CEventWaitHandle::Set()
{
	return SetEvent(this->eventHandle) != 0;
}

/// Resets the Event
bool CEventWaitHandle::Reset()
{
	return ResetEvent(this->eventHandle) != 0;
}

/// Waits a specified time (in milliseconds)
DWORD CEventWaitHandle::Wait(int milliseconds)
{
	return WaitForSingleObject(this->eventHandle, milliseconds);
}

/// Waits until the event is signaled
DWORD CEventWaitHandle::Wait()
{
	return CEventWaitHandle::Wait(INFINITE);
}

HANDLE CEventWaitHandle::GetEventHandle()
{
	return this->eventHandle;
}

/// Opens the Event with the specified name.
CEventWaitHandle *OpenEventWaitHandle(char *name)
{
	CEventWaitHandle *handle = new CEventWaitHandle(name);

	if (handle->GetEventHandle() == nullptr) {
		delete (void *)handle;
		return nullptr;
	}

	return handle;
}

/// Returns the full name for an event number
char *GetEventName(int eventName)
{
	char *evName = new char[128];
	sprintf_s(evName, 128, "Local\\Profiler.Event.%d", eventName);
	
	return evName;
}
