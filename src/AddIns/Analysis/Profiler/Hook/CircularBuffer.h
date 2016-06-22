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