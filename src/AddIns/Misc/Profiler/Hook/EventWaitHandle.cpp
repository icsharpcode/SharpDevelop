// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
