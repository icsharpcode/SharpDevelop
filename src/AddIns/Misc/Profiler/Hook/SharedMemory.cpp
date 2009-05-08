// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "SharedMemory.h"

CSharedMemory::CSharedMemory(char *name)
{
	this->fileHandle = OpenFileMapping(FILE_MAP_ALL_ACCESS, false, name);
	if (fileHandle == nullptr) {
		DebugWriteLine(L"OpenFileMapping returned nullptr");
	}
	this->startPtr = MapViewOfFile(this->fileHandle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(SharedMemoryHeader));
	if (startPtr == nullptr) {
		DebugWriteLine(L"MapViewOfFile returned nullptr");
		MessageBox(nullptr, TEXT("Could not open Shared Memory, please restart the profiler!"), TEXT("Profiler Error"), MB_OK);
	}
	SharedMemoryHeader *header = (SharedMemoryHeader*)this->startPtr;
	#ifdef DEBUG
	if (header->Magic != '~DBG') {
		DebugWriteLine(L"Corrupted shared memory header");
		if (header->Magic == '~SM1') {
			MessageBox(nullptr, TEXT("Wrong build configuration; DEBUG needed!"), TEXT("Profiler Error"), MB_OK);
		}
	}
	#else
	if (header->Magic != '~SM1') {
		DebugWriteLine(L"Corrupted shared memory header");
		if (header->Magic == '~DBG') {
			MessageBox(nullptr, TEXT("Wrong build configuration; RELEASE needed!"), TEXT("Profiler Error"), MB_OK);
		}
	}
	#endif
	this->length = header->TotalLength;
	UnmapViewOfFile(this->startPtr);
	DebugWriteLine(L"Length: %d", this->length);
	this->startPtr = MapViewOfFile(this->fileHandle, FILE_MAP_ALL_ACCESS, 0, 0, this->length);
	if (startPtr == nullptr) {
		char buffer[512];
		sprintf_s(buffer, 512, "Error while creating temporary storage file (shared memory)!\n\nError: %d", GetLastError());
		DebugWriteLine(L"second MapViewOfFile returned nullptr");
		MessageBox(nullptr, buffer, TEXT("Profiler Error"), MB_OK);
	}
	this->header = (SharedMemoryHeader*)this->startPtr;
}

CSharedMemory::~CSharedMemory(void)
{
	CloseHandle(this->fileHandle);
}

// Gets the memory address at which the file is mapped
void* CSharedMemory::GetStartPtr()
{
	return this->startPtr;
}