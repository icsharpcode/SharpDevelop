// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "CircularBuffer.h"
#include "SharedMemory.h"

CCircularBuffer::CCircularBuffer(void *start)
{
	this->header = (CircularBufferHeader*)start;
	if (this->header->Magic != '~CB1') {
		DebugWriteLine(L"Corrupted buffer header");
	}
	this->data = (byte*)(this->header + 1);
	this->dataLength = this->header->TotalLength - sizeof(CircularBufferHeader);
	this->nonFullEvent = OpenEventWaitHandle(GetEventName(this->header->NonFullEventName));
	this->nonEmptyEvent = OpenEventWaitHandle(GetEventName(this->header->NonEmptyEventName));

	if (this->nonFullEvent != nullptr)
		DebugWriteLine(L"nonFullEvent found!\n");
	if (this->nonEmptyEvent != nullptr)
		DebugWriteLine(L"nonEmptyEvent found!\n");

	DebugWriteLine(L"circular buffer header: %x %x %x %x\n", this->header->StartOffset, this->header->EndOffset, this->header->NonEmptyEventName, this->header->NonFullEventName);
}

int CCircularBuffer::Read(byte *data, int offset, int count)
{
	if (data == nullptr)
		return -1;
	if (count < 0)
		return -2;
	if (offset < 0)
		return -3;

	DebugWriteLine(L"\ninside read!\n");
	
	// at least read one byte
	if (count == 0)
		return 0;

	// read StartOffset and endOffset from shared memory
	int startOffset = this->header->StartOffset;
	int endOffset = this->header->EndOffset;
	// wait until there's data
	while (startOffset == endOffset) {
		if (this->nonEmptyEvent->Wait() == WAIT_TIMEOUT)
			return -4;
		// the writer should have changed the end offset
		endOffset = this->header->EndOffset;
	}

	if (endOffset < startOffset) {
		// data wraps over the buffer end => read only until buffer end
		endOffset = this->dataLength; // change only our local copy of endOffset
	}
	int readCount = min(count, endOffset - startOffset);
	
	DebugWriteLine(L"read amount %d\n", readCount);
	
	memcpy(data + offset, this->data + startOffset, readCount);
	
	startOffset += readCount; 	   // advance startOffset
	if (startOffset == this->dataLength) // wrap around startOffset if required
		startOffset = 0;
	this->header->StartOffset = startOffset; // write back startOffset to shared memory
	this->nonFullEvent->Set(); // we read something, so the buffer is not full anymore
	return readCount;
}

int CCircularBuffer::Read(byte* buffer, int count)
{
  return Read(buffer, 0, count);
}

byte CCircularBuffer::ReadByte()
{
  byte ret;
  Read(&ret, 0, 1);
  return ret;
}

int CCircularBuffer::Write(byte *data, int offset, int count)
{
	if (data == nullptr)
		return -1;
	if (count < 0)
		return -2;
	if (offset < 0)
		return -3;
	
	while (count > 0) {
		int r = WriteInternal(data, offset, count);
		offset += r;
		count -= r;
	}
	return count;
}

int CCircularBuffer::Write(byte *data, int count)
{
	return Write(data, 0, count);
}

int CCircularBuffer::WriteByte(byte data)
{
	return Write(&data, 0, 1);
}

int CCircularBuffer::WriteInt(int data)
{
	return Write((byte*)&data, 0, 4);
}

int CCircularBuffer::WriteString(WCHAR *string)
{
	int len = (int)(wcslen(string) * sizeof(WCHAR));
	WriteInt(len);
	return Write((byte *)string, 0, len);
}

int CCircularBuffer::WriteInternal(byte *data, int offset, int count)
{
	// read startOffset and endOffset from shared memory
	int startOffset = this->header->StartOffset;
	int endOffset = this->header->EndOffset;
	// wait until there's room
	while (NextOffset(endOffset) == startOffset) {
		if (this->nonFullEvent->Wait() == WAIT_TIMEOUT)
			return -1;
		// the reader should have changed the start offset
		startOffset = this->header->StartOffset;
	}
	int writeEndOffset;
	if (startOffset <= endOffset) {
		// free space wraps over buffer end
		if (startOffset == 0)
			writeEndOffset = this->dataLength - 1; // one byte must always be left free
		else
			writeEndOffset = this->dataLength;
	} else {
		writeEndOffset = startOffset - 1; // one byte must be left free to distinguish between empty and full buffer
	}
	int writeCount = min(count, writeEndOffset - endOffset);
	
	memcpy(this->data + endOffset, data + offset, writeCount);
	endOffset += writeCount; // advance endOffset
	if (endOffset == this->dataLength) // wrap around startOffset if required
		endOffset = 0;

	this->header->EndOffset = endOffset; // write back endOffset to shared memory
	this->nonEmptyEvent->Set(); // we wrote something, so the buffer is not empty anymore
	return writeCount;
}

int CCircularBuffer::NextOffset(int offset)
{
	offset += 1;
	if (offset == this->dataLength)
		return 0;
	else
		return offset;
}