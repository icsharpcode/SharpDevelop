// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma once

struct FunctionInfo
{
	int Id;
	int CallCount;
	// TimeSpent: first 8 bits: index in parent
	//            following bit: function was called in previous dataset
	//            other 55 bits: number of CPU cycles
	ULONGLONG TimeSpent;
	int FillCount; // Number of elements in the hash table
	int LastChildIndex;        // (Capacity - 1)  Binary value in the from 000001111 for some number of ones
	                           // Therefore it can be used as bit mask
	FunctionInfo* Children[0]; // Inline array.  Must be the last element.
	                           // Overruns behind the struct.  The size must be power of 2.
	                           // Elements must point either to valid FunctionInfo or be a null pointer.
	
	// Gets a child, or allocates a new one.
	// This method can cause the parent FunctionInfo to be moved in memory!
	// If this FunctionInfo was moved, newParent will point to the new location.
	// If this FunctionInfo was not moved, the value of newParent is not changed.
	// Note that if the FunctionInfo is moved, the caller is responsible for freeing the old FunctionInfo!
	FunctionInfo* GetOrAddChild(int id, FunctionInfo*& newParent);

	// Adds the child or updates an existing child.
	// Does not resize the table, you can run into an endless loop if you fill the table completely!
	void AddOrUpdateChild(FunctionInfo *child);
	
	// checks if the data structure is valid
	void Check();

private:
	// Resizes the datastructure.  The new instance will be located at different memory address 
	FunctionInfo* Resize(int newTableSize);
};

FunctionInfo* CreateFunctionInfo(int id, int indexInParent);
void FreeFunctionInfo(FunctionInfo* f);
