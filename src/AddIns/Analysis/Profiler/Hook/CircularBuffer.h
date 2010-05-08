// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#pragma once

#include "EventWaitHandle.h"
#include "SharedMemory.h"

struct CircularBufferHeader
{
	int Magic;       // Verification value - always '~CB1'
	int TotalLength; // Length including the header
	volatile int StartOffset; // only the reader is writing to this variable
	volatile int EndOffset;   // only the writer is writing to this variable
	int NonEmptyEventName;
	int NonFullEventName;
};

class CCircularBuffer
{
public:
	// Constructors
	CCircularBuffer(void* start);

	// Methods
	int Write(byte *data, int offset, int count);
	int Write(byte *data, int count);
	int WriteByte(byte data);
	int WriteInt(int data);
	int WriteString(WCHAR *string);

	int Read(byte *data, int offset, int count);
	int Read(byte *data, int count);
	byte ReadByte();
private:
	// Fields
	CircularBufferHeader *header;
	byte *data;
	int dataLength;

	CEventWaitHandle *nonEmptyEvent;
	CEventWaitHandle *nonFullEvent;

	// Helpers
	int NextOffset(int offset);
	int WriteInternal(byte *data, int offset, int count);
};