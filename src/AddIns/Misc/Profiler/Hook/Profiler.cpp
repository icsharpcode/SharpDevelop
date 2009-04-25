// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "global.h"
#include "Profiler.h"
#include "windows.h"
#include "CircularBuffer.h"
#include "allocator.h"
#include "FunctionInfo.h"
#include "LightweightList.h"
#include "Callback.h"
#include <cassert>
#include <intrin.h>
#include "ProfilerMetaData.h"

STDMETHODIMP_(ULONG) CProfiler::AddRef() 
{
	return 1;
}

STDMETHODIMP_(ULONG) CProfiler::Release() 
{
	return 1;
}

HRESULT CProfiler::QueryInterface(const IID &riid, LPVOID *ppv)
{
  *ppv = nullptr;
  if(IsEqualIID(riid, IID_IUnknown) ||
	 IsEqualIID(riid, IID_ICorProfilerCallback) ||
	 IsEqualIID(riid, IID_ICorProfilerCallback2)) {

    *ppv = this;  

    return S_OK;
  }
  return E_NOINTERFACE;
}

// ----  CALLBACK FUNCTIONS ------------------

__declspec(noinline) void FunctionEnterCreateNewRoot(ThreadLocalData *data, const ULONGLONG &tsc)
{
	DebugWriteLine(L"Creating new root");
	// first function call on this thread
	// create a new root node for this thread
	data->stack.push(StackEntry(profiler.CreateNewRoot(), tsc));
	DebugWriteLine(L"Created new root");
}

void CProfiler::EnterLock(ThreadLocalData *data)
{
	data->inLock = 1;
	_ReadWriteBarrier(); _mm_mfence();

	while (sharedMemoryHeader->ExclusiveAccess == 1)
	{
		data->inLock = 0;
		this->accessEventHandle->Wait();
		data->inLock = 1;
		_ReadWriteBarrier(); _mm_mfence();
	}
}

ASSEMBLER_CALLBACK FunctionEnterGlobal(int functionID)
{
	ThreadLocalData *data = getThreadLocalData();
	
	if (data == nullptr) {
		data = AttachToThread();
		profiler.EnterLock(data);
		FunctionEnterCreateNewRoot(data, __rdtsc());
	} else
		profiler.EnterLock(data);

	// this call allows GetOrAddChild to update the value at the top of the stack
	// if the FunctionInfo is resized
	FunctionInfo *f = data->stack.top().function;
	
	if ((sharedMemoryHeader->doNotProfileDotnetInternals && functionID < 0 && f->Id < 0) || (sharedMemoryHeader->combineRecursiveFunction && functionID == f->Id)) {
		data->stack.top().frameCount++;
	} else {
		//DebugWriteLine(L"FunctionEnterGlobal %d, current stack top=%d", functionID, f->Id);
		FunctionInfo *newParent = nullptr;
		FunctionInfo *child = f->GetOrAddChild(functionID, newParent);
		if (newParent != nullptr) {
			// f was moved to newParent
			// update stack:
			data->stack.top().function = newParent;
			// update parent of f:
			if (data->stack.hasAtLeastTwoElements()) {
				//DebugWriteLine("Updating parent of parent");
				data->stack.belowTop().function->AddOrUpdateChild(newParent);
				data->stack.belowTop().function->Check();
			} else {
				DebugWriteLine(L"Updating parent of parent (root)");
				profiler.MovedRootChild(newParent);
			}
			FreeFunctionInfo(f);
		}
		
		// Set the stats for this function
		child->CallCount++;
		
		data->stack.push(StackEntry(child, __rdtsc()));
	}

	data->inLock = 0;
}

void DetachFromThread(ThreadLocalData *data)
{
	if (data != nullptr) {
		DebugWriteLine(L"DetachFromThread %d", data->threadID);
		ULONGLONG tsc = __rdtsc();
		while (!data->stack.empty()) {
			StackEntry &stackTop = data->stack.top();
			stackTop.function->TimeSpent += (tsc - stackTop.startTime);
			data->stack.pop();
		}
	}
}

ASSEMBLER_CALLBACK FunctionLeaveGlobal()
{
	ThreadLocalData *data = getThreadLocalData();

	profiler.EnterLock(data);

	if (data->stack.empty()) {
		//DebugWriteLine(L"FunctionLeaveGlobal (but stack was empty)");
	} else {
		StackEntry &stackTop = data->stack.top();
		//DebugWriteLine(L"FunctionLeaveGlobal %d", stackTop.function->Id);
		stackTop.frameCount--;
		if (stackTop.frameCount == 0) {
			stackTop.function->TimeSpent += (__rdtsc() - stackTop.startTime);
			data->stack.pop();
		}
	}

	data->inLock = 0;
}

ASSEMBLER_CALLBACK FunctionTailcallGlobal()
{
	DebugWriteLine(L"FunctionTailcallGlobal");
	// handle tail calls A->B as leave A, enter B, ...
	FunctionLeaveGlobal();
	
	// FunctionTailcallGlobal call will be followed by FunctionEnterGlobal for new function
}

volatile LONG nextPosFunctionID = 0;

int getNewPosFunctionID() {
	const int step = 5;
	// Simple continuous assignment of IDs leads to problems with the
	// linear probing in FunctionInfo, so we use multiples of 'step' as IDs.
	// The step should not be a multiple of 2 (otherwise parts of the hash table could not be used)!
	LONG oldID;
	LONG newID;
	do {
		oldID = nextPosFunctionID;
		newID = ((oldID + step) & 0x7FFFFFFF); // x % 2^31
	} while (InterlockedCompareExchange(&nextPosFunctionID, newID, oldID) != oldID);
	return newID;
}

// this function is called by the CLR when a function has been mapped to an ID
UINT_PTR CProfiler::FunctionMapper(FunctionID functionID, BOOL *)
{
	return profiler.MapFunction(functionID);
}

UINT_PTR CProfiler::MapFunction(FunctionID functionID)
{
	mapFunctionCriticalSection.Enter();
	int clientData = 0;
	
	TFunctionIDMap::iterator it = this->functionIDMap.find(functionID);
	if (it == this->functionIDMap.end()) {
		DebugWriteLine(L"Creating new ID");
		if (sigReader->IsNetInternal(functionID))
			clientData = -getNewPosFunctionID(); // negative series
		else
			clientData = getNewPosFunctionID(); // positive series
		this->functionIDMap.insert(TFunctionIDPair(functionID, clientData));

		// send to host
		std::wstring signature = sigReader->Parse(functionID);
		LogString(L"map %d %Id %s", clientData, functionID, signature.c_str());
	} else {
		DebugWriteLine(L"using old ID");
		clientData = it->second;
	}
	mapFunctionCriticalSection.Leave();
	
	return clientData;
}

FunctionInfo *CProfiler::CreateNewRoot() {
	rootElementCriticalSection.Enter();
	FunctionInfo *oldRoot = sharedMemoryHeader->RootFuncInfo;
	FunctionInfo *newRoot = nullptr;
	FunctionInfo *newThreadRoot = oldRoot->GetOrAddChild(getNewPosFunctionID(), newRoot);
	if (newRoot != nullptr) {
		sharedMemoryHeader->RootFuncInfo = newRoot;
		FreeFunctionInfo(oldRoot);
	}
	rootElementCriticalSection.Leave();
	
	ThreadInfo *data = this->unmanagedThreadIDMap[GetCurrentThreadId()];
	
	data->functionInfoId = newThreadRoot->Id;
		
	LogString(L"mapthread %d 0 \"Thread#%d\" \"%s\"", newThreadRoot->Id, GetCurrentThreadId(), data->threadName.c_str());
	
	return newThreadRoot;
}

void CProfiler::MovedRootChild(FunctionInfo *newRootChild)
{
	rootElementCriticalSection.Enter();
	sharedMemoryHeader->RootFuncInfo->AddOrUpdateChild(newRootChild);
	rootElementCriticalSection.Leave();
}

CProfiler::CProfiler()
{
	this->pICorProfilerInfo = nullptr;
	this->pICorProfilerInfo2 = nullptr;
	this->sigReader = nullptr;
}

// ----  ICorProfilerCallback IMPLEMENTATION ------------------

// called when the profiling object is created by the CLR
STDMETHODIMP CProfiler::Initialize(IUnknown *pICorProfilerInfoUnk)
{
	#ifdef DEBUG
	MessageBox(nullptr, TEXT("CProfiler::Initialize - Attach debugger now!"), TEXT("Attach debugger"), MB_OK);
	// __debugbreak(); // __asm int 3; replacement on x64 and Itanium
	#endif

	// Have to disable the profiler, if this process starts other .NET processes (e. g. run a project in SharpDevelop)
	SetEnvironmentVariable("COR_ENABLE_PROFILING", "0");
	
	InitializeCommunication();

	// log that we are initializing
	LogString(L"Initializing...");
	
	sharedMemoryHeader->mallocator.initialize((byte *)sharedMemory->GetStartPtr() + sharedMemoryHeader->HeapOffset, (byte *)sharedMemory->GetStartPtr() + sharedMemoryHeader->HeapOffset + sharedMemoryHeader->HeapLength);
	
	stackAllocator.initialize((byte *)sharedMemory->GetStartPtr() + sharedMemoryHeader->ThreadDataOffset, (byte *)sharedMemory->GetStartPtr() + sharedMemoryHeader->ThreadDataOffset + sharedMemoryHeader->ThreadDataLength);

	sharedMemoryHeader->RootFuncInfo = CreateFunctionInfo(0, 0);

	// get the ICorProfilerInfo interface
    HRESULT hr = pICorProfilerInfoUnk->QueryInterface(IID_ICorProfilerInfo, (LPVOID*)&pICorProfilerInfo);
    if (FAILED(hr))
        return E_FAIL;
	// determine if this object implements ICorProfilerInfo2
    hr = pICorProfilerInfoUnk->QueryInterface(IID_ICorProfilerInfo2, (LPVOID*)&pICorProfilerInfo2);
    if (FAILED(hr))
	{
		LogString(L"FATAL ERROR: Unsupported .NET version needs to be at least 2.0!");
		this->pICorProfilerInfo2 = nullptr;
		return E_FAIL;
	}

	// Indicate which events we're interested in.
	hr = SetEventMask();
    if (FAILED(hr))
        LogString(L"Error setting the event mask");

	// set the enter, leave and tailcall hooks
	hr = pICorProfilerInfo2->SetEnterLeaveFunctionHooks2(FunctionEnterNaked, FunctionLeaveNaked, FunctionTailcallNaked);
	if (SUCCEEDED(hr))
		hr = pICorProfilerInfo2->SetFunctionIDMapper(FunctionMapper);
	// report our success or failure to the log file
    if (FAILED(hr))
        LogString(L"Error setting the enter, leave and tailcall hooks");
	else
		LogString(L"Successfully initialized profiling" );

	this->sigReader = new SignatureReader(this->pICorProfilerInfo);
	
    return S_OK;
}

// called when the profiler is being terminated by the CLR
STDMETHODIMP CProfiler::Shutdown()
{
	LogString(L"Shutdown...");

    return S_OK;
}

int CProfiler::InitializeCommunication()
{	
	DebugWriteLine(L"Looking for Shared Memory...");
	TCHAR sharedMemName[68];
	memset(sharedMemName, 0, sizeof(sharedMemName));
	if (GetEnvironmentVariable("SharedMemoryName", sharedMemName, 68) == 0) {
		DebugWriteLine(L"Getting environment variable failed");
		if (GetLastError() == ERROR_ENVVAR_NOT_FOUND)
			MessageBox(nullptr, TEXT("Could not find environment variable 'SharedMemoryName', please restart the profiler!"), TEXT("Profiler Error"), MB_OK);
		else
			MessageBox(nullptr, TEXT("Unknown error!"), TEXT("Profiler Error"), MB_OK);

		return 1;
	}

	DebugWriteLine(L"Got Shared Memory...");
	this->sharedMemory = new CSharedMemory(sharedMemName);
	sharedMemoryHeader = (SharedMemoryHeader*)sharedMemory->GetStartPtr();
	
	this->nativeToManagedBuffer = new CCircularBuffer((byte *)sharedMemory->GetStartPtr() + sharedMemoryHeader->NativeToManagedBufferOffset);
	
	sharedMemoryHeader->TargetPointer = this->sharedMemory->GetStartPtr();
	
	TCHAR mutexName[61];
	memset(mutexName, 0, sizeof(mutexName));
	
	if (GetEnvironmentVariable("MutexName", mutexName, 61) == 0) {
		DebugWriteLine(L"Getting environment variable failed");
		if (GetLastError() == ERROR_ENVVAR_NOT_FOUND)
			MessageBox(nullptr, TEXT("Could not find environment variable 'MutexName', please restart the profiler!"), TEXT("Profiler Error"), MB_OK);
		else
			MessageBox(nullptr, TEXT("Unknown error!"), TEXT("Profiler Error"), MB_OK);

		return 1;
	}

	HANDLE mutex = OpenMutex(MUTEX_ALL_ACCESS, false, mutexName);

	if (mutex == nullptr) {
		DebugWriteLine(L"Failed to access mutex: %d!", GetLastError());

		return 1;
	}
	
	DebugWriteLine(L"Mutex opened successfully!");

	allThreadLocalDatas = new LightweightList(mutex);

	TCHAR accessEventName[62];
	memset(accessEventName, 0, sizeof(accessEventName));

	if (GetEnvironmentVariable("AccessEventName", accessEventName, 62) == 0) {
		DebugWriteLine(L"Getting environment variable failed");
		if (GetLastError() == ERROR_ENVVAR_NOT_FOUND)
			MessageBox(nullptr, TEXT("Could not find environment variable 'AccessEventName', please restart the profiler!"), TEXT("Profiler Error"), MB_OK);
		else
			MessageBox(nullptr, TEXT("Unknown error!"), TEXT("Profiler Error"), MB_OK);

		return 1;
	}

	this->accessEventHandle = new CEventWaitHandle(accessEventName);

	return 0;
}

// Writes a string to the log file.  Uses the same calling convention as printf.
void CProfiler::LogString(WCHAR *pszFmtString, ...)
{
	WCHAR szBuffer[2 * 4096];

	va_list args;
	va_start( args, pszFmtString );
	vswprintf_s(szBuffer, 2 * 4096, pszFmtString, args );
	va_end( args );
	
	DebugWriteLine(szBuffer);
	nativeToManagedCriticalSection.Enter();
	nativeToManagedBuffer->WriteString(szBuffer);
	nativeToManagedCriticalSection.Leave();
}

HRESULT CProfiler::SetEventMask()
{
	DWORD eventMask = COR_PRF_MONITOR_ENTERLEAVE | COR_PRF_MONITOR_THREADS | COR_PRF_MONITOR_FUNCTION_UNLOADS | COR_PRF_MONITOR_CLR_EXCEPTIONS | COR_PRF_MONITOR_EXCEPTIONS;
	return pICorProfilerInfo->SetEventMask(eventMask);
}

// THREAD CALLBACK FUNCTIONS
STDMETHODIMP CProfiler::ThreadAssignedToOSThread(ThreadID managedThreadID, DWORD osThreadID)
{
	this->threadMapCriticalSection.Enter();
	DebugWriteLine(L"ThreadAssignedToOSThread %d, %d", managedThreadID, osThreadID);
	
	TThreadIDMap::iterator it = this->threadIDMap.find(managedThreadID);
	TUThreadIDMap::iterator it2 = this->unmanagedThreadIDMap.find(osThreadID);
	
	if (it != this->threadIDMap.end() && it2 == this->unmanagedThreadIDMap.end()) {
		ThreadInfo *data = it->second;
		
		data->managedThreadId = managedThreadID;
		data->unmanagedThreadId = osThreadID;
		
		this->unmanagedThreadIDMap.insert(TUThreadIDPair(osThreadID, data));
	} else {
		DebugWriteLine(L"Thread %d (%d) already exists in map!", managedThreadID, osThreadID);
		LogString(L"error-Thread %d (%d) already exists in map!-", managedThreadID, osThreadID);
		return E_FAIL;
	}
	
	this->threadMapCriticalSection.Leave();
	return S_OK;
}

STDMETHODIMP CProfiler::ThreadNameChanged(ThreadID threadID, ULONG cchName, WCHAR name[])
{
	this->threadMapCriticalSection.Enter();
	DebugWriteLine(L"ThreadNameChanged %d, %s", threadID, name);
	
	TThreadIDMap::iterator it = this->threadIDMap.find(threadID);
	
	ThreadInfo *data;
	
	if (it != this->threadIDMap.end()) {
		DebugWriteLine(L"Thread %d, %s exists", threadID, name);
		data = it->second;
		
		data->managedThreadId = threadID;
		data->threadName.assign(name, cchName);
	} else {
		DebugWriteLine(L"Thread %d, %s does not exist", threadID, name);
		data = new ThreadInfo();
		
		data->managedThreadId = threadID;
		data->threadName.assign(name, cchName);
		
		this->threadIDMap.insert(TThreadIDPair(threadID, data));
	}
			
	this->threadMapCriticalSection.Leave();
	return S_OK;
}

// UNLOAD CALLBACK FUNCTIONS
STDMETHODIMP CProfiler::FunctionUnloadStarted(FunctionID functionID)
{
	mapFunctionCriticalSection.Enter();
	DebugWriteLine(L"FunctionUnloadStarted %d", functionID);
	
	this->functionIDMap.erase(functionID);
	mapFunctionCriticalSection.Leave();
    return S_OK;
}

STDMETHODIMP CProfiler::ThreadCreated(ThreadID threadID)
{
	this->threadMapCriticalSection.Enter();
	DebugWriteLine(L"ThreadCreated %d", threadID);
	
	TThreadIDMap::iterator it = this->threadIDMap.find(threadID);
	
	if (it == this->threadIDMap.end()) {
		ThreadInfo *data = new ThreadInfo();
		
		data->managedThreadId = threadID;
		
		this->threadIDMap.insert(TThreadIDPair(threadID, data));
	} else {
		DebugWriteLine(L"ThreadCreated %d - did not create thread (already exists)", threadID);
	}
	
	this->threadMapCriticalSection.Leave();
	return S_OK;
}

STDMETHODIMP CProfiler::ThreadDestroyed(ThreadID threadID)
{
	this->threadMapCriticalSection.Enter();
	DebugWriteLine(L"ThreadDestroyed %d", threadID);
	
	TThreadIDMap::iterator it = this->threadIDMap.find(threadID);
	
	if (it != this->threadIDMap.end()) {
		ThreadInfo *data = it->second;
		
		this->unmanagedThreadIDMap.erase(data->unmanagedThreadId);
		this->threadIDMap.erase(threadID);
		
		delete data;
	}
	
	this->threadMapCriticalSection.Leave();
	return S_OK;
}

STDMETHODIMP CProfiler::ExceptionThrown(ObjectID)
{
	DebugWriteLine(L"ExceptionThrown");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionUnwindFunctionEnter(FunctionID)
{
	DebugWriteLine(L"ExceptionUnwindFunctionEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionUnwindFunctionLeave()
{
	DebugWriteLine(L"ExceptionUnwindFunctionLeave");
	FunctionLeaveGlobal();
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionSearchFunctionEnter(FunctionID)
{
	DebugWriteLine(L"ExceptionSearchFunctionEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionSearchFunctionLeave()
{
	DebugWriteLine(L"ExceptionSearchFunctionLeave");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionSearchFilterEnter(FunctionID)
{
	DebugWriteLine(L"ExceptionSearchFilterEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionSearchFilterLeave()
{
	DebugWriteLine(L"ExceptionSearchFilterLeave");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionSearchCatcherFound(FunctionID)
{
	DebugWriteLine(L"ExceptionSearchCatcherFound");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionCLRCatcherFound()
{
	DebugWriteLine(L"ExceptionCLRCatcherFound");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionCLRCatcherExecute()
{
	DebugWriteLine(L"ExceptionCLRCatcherExecute");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionOSHandlerEnter(FunctionID)
{
	DebugWriteLine(L"ExceptionOSHandlerEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionOSHandlerLeave(FunctionID)
{
	DebugWriteLine(L"ExceptionOSHandlerLeave");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionUnwindFinallyEnter(FunctionID)
{
	DebugWriteLine(L"ExceptionUnwindFinallyEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionUnwindFinallyLeave()
{
	DebugWriteLine(L"ExceptionUnwindFinallyLeave");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionCatcherEnter(FunctionID, ObjectID)
{
	DebugWriteLine(L"ExceptionCatcherEnter");
    return S_OK;
}

STDMETHODIMP CProfiler::ExceptionCatcherLeave()
{
	DebugWriteLine(L"ExceptionCatcherLeave");
    return S_OK;
}