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
	this->header = (SharedMemoryHeader*)this->startPtr;
	if (this->header->Magic != '~SM1') {
		DebugWriteLine(L"Corrupted shared memory header");
	}
	this->length = this->header->TotalLength;
	UnmapViewOfFile(this->startPtr);
	this->startPtr = MapViewOfFile(this->fileHandle, FILE_MAP_ALL_ACCESS, 0, 0, this->length);
	if (startPtr == nullptr) {
		DebugWriteLine(L"second MapViewOfFile returned nullptr");
	}
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