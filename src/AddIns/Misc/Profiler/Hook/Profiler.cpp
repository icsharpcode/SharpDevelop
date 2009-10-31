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
		
	if (!data->active)
		goto EXIT;

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
EXIT:
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
	
	if (!data->active)
		goto EXIT;

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
	
EXIT:
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
	return profiler.MapFunction(functionID, nullptr);
}

UINT_PTR CProfiler::MapFunction(FunctionID functionID, const WCHAR **sigOutput)
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
		if (sigOutput != nullptr)
			*sigOutput = sigReader->fullName.c_str();
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
	//__debugbreak();
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
	DWORD eventMask = COR_PRF_MONITOR_ENTERLEAVE | COR_PRF_MONITOR_THREADS | 
		COR_PRF_MONITOR_FUNCTION_UNLOADS | COR_PRF_MONITOR_CLR_EXCEPTIONS |
		COR_PRF_MONITOR_EXCEPTIONS | COR_PRF_MONITOR_JIT_COMPILATION |
		COR_PRF_MONITOR_MODULE_LOADS;
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

STDMETHODIMP CProfiler::JITCompilationStarted(FunctionID functionID, BOOL /*fIsSafeToBlock*/)
{
	WCHAR *name;
	int nameId = (int)MapFunction(functionID, (const WCHAR **)(&name));
	
	for (int i = 0; i < CONSOLE_GROUP_LENGTH; i++) {
		if (wcsstr(consoleGroupList[i], name) != nullptr)
			Rewrite(functionID, 0x1, nameId);
	}
	
	for (int i = 0; i < WINFORMS_GROUP_LENGTH; i++) {
		if (wcsstr(winFormsGroupList[i], name) != nullptr)
			Rewrite(functionID, 0x2, nameId);
	}
	
	for (int i = 0; i < WPF_GROUP_LENGTH; i++) {
		if (wcsstr(wpfGroupList[i], name) != nullptr)
			Rewrite(functionID, 0x3, nameId);
	}

	return S_OK;
}

void CProfiler::Activate()
{
	ThreadLocalData *data = getThreadLocalData();
	data->active = true;
}

void CProfiler::Deactivate()
{
	ThreadLocalData *data = getThreadLocalData();
	data->active = false;
}

const ULONG FAT_HEADER_SIZE = 0xC;     /* 12 bytes = WORD + WORD + DWORD + DWORD */
const ULONG TINY_HEADER_SIZE = 0x1;    /* 1 byte */
const ULONG MAX_CODE_SIZE_TINY = 0x40; /* 64 bytes */
const WORD  DEFAULT_MAX_STACK = 0x8;   /* default stack depth in slots */

void CProfiler::Rewrite(FunctionID functionID, int type, int nameId)
{
	ModuleID moduleID;
	mdToken token;
	HRESULT hr = S_OK;
	
	hr = pICorProfilerInfo2->GetFunctionInfo(functionID, 0, &moduleID, &token);
	
	if (!SUCCEEDED(hr))
		return;
	
	LPCBYTE header;
	ULONG length;

	hr = pICorProfilerInfo2->GetILFunctionBody(moduleID, token, &header, &length);
	
	if (!SUCCEEDED(hr))
		return;
		
	IMethodMalloc *allocator = nullptr;
	
	hr = pICorProfilerInfo2->GetILFunctionBodyAllocator(moduleID, &allocator);
	
	if (!SUCCEEDED(hr) || allocator == nullptr)
		return;
	
	methodMapCriticalSection.Enter();
	TMethodTokenMap::iterator it = this->methodMap.find(moduleID);
	
	mdMethodDef deactivateCall = nullptr;
	mdMethodDef activateCall = nullptr;
	mdMethodDef loggerCall = nullptr;
	
	if (it != this->methodMap.end()) {
		deactivateCall = it->second.deactivator;
		activateCall = it->second.activator;
		loggerCall = it->second.logger;
	}
	
	methodMapCriticalSection.Leave();
	
	if (deactivateCall == nullptr || activateCall == nullptr || loggerCall == nullptr)
		return;
		
	IMetaDataImport *metaData = nullptr;
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead, IID_IMetaDataImport, (LPUNKNOWN *) &metaData);

	if (!SUCCEEDED(hr) || metaData == nullptr)
		return;

	byte *codeBuf = 0;
	
	codeBuf = (byte *)allocator->Alloc(length + 64);
    allocator->Release(); 
	
	if (codeBuf == nullptr)
		return;
	
	int newLength = 0;

	if (((COR_ILMETHOD_TINY *)header)->IsTiny()) {
	    COR_ILMETHOD_TINY *method = (COR_ILMETHOD_TINY *)header;

		if (method->GetCodeSize() + sizeof(activateCall) + sizeof(loggerCall) + sizeof(deactivateCall) + 2 < MAX_CODE_SIZE_TINY) {
			// Copy the header elements.
			memcpy(&codeBuf[0], method, TINY_HEADER_SIZE);
			
			newLength = TINY_HEADER_SIZE;
			
			SetInjectionCode(metaData, codeBuf, &newLength, activateCall, loggerCall, deactivateCall, type, nameId);
			
			// copy old code
			memcpy(&codeBuf[newLength],
					&header[TINY_HEADER_SIZE],
					length - TINY_HEADER_SIZE);
			
			newLength += (length - TINY_HEADER_SIZE);
			
			// update code size in header
			// LLLLLLUU
			// 2 upper bits are the tiny header 6 lower bits are length
			codeBuf[0] = (byte)((newLength - 1) << 2 | 0x2); 
        } else {
			ConvertToFat((byte *)codeBuf, &newLength);
		
			SetInjectionCode(metaData, codeBuf, &newLength, activateCall, loggerCall, deactivateCall, type, nameId);
						
			// copy old code
			memcpy(&codeBuf[newLength], &header[TINY_HEADER_SIZE], length - TINY_HEADER_SIZE);
			newLength += (length - TINY_HEADER_SIZE);
			
			COR_ILMETHOD_FAT *target = (COR_ILMETHOD_FAT *)codeBuf;
			target->CodeSize = newLength - FAT_HEADER_SIZE;
        }
	} else if (((COR_ILMETHOD_FAT *)header)->IsFat()) {
		COR_ILMETHOD_FAT *method = (COR_ILMETHOD_FAT *)header;
		COR_ILMETHOD_FAT *target = (COR_ILMETHOD_FAT *)codeBuf;
		
		// Copy the header elements.
		newLength = FAT_HEADER_SIZE;
		memcpy(&codeBuf[0], method, newLength);

		int diff = newLength;
		
		// insert new code
		SetInjectionCode(metaData, codeBuf, &newLength, activateCall, loggerCall, deactivateCall, type, nameId);

		diff = newLength - diff;
		
		// Copy the remainder of the method body.
		memcpy(&codeBuf[newLength], &header[FAT_HEADER_SIZE], length - FAT_HEADER_SIZE);
		newLength += (length - FAT_HEADER_SIZE);
		
		// Reset the size of the code in the header itself.
		target->CodeSize = newLength - FAT_HEADER_SIZE;
		
		// Increase MaxStack to allow our injection code to run.
		target->MaxStack = (method->MaxStack > 8) ? method->MaxStack : 8;
		
		// Fix SEH sections
		FixSEHSections(target->GetSect(), diff); 
    } else {
    	DebugWriteLine(L"neither fat nor tiny ... invalid header???");
    }
    
    if (codeBuf != nullptr && newLength > 0) {
        hr = pICorProfilerInfo2->SetILFunctionBody(moduleID, token, codeBuf);
    }
}

void CProfiler::SetInjectionCode(IMetaDataImport *metaData, byte *buffer, int *size,
									mdMethodDef activateCall, mdMethodDef loggerCall, mdMethodDef deactivateCall,
									int type, int nameId)
{
	HRESULT hr = S_OK;

	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = deactivateCall;
	*size += sizeof(deactivateCall);
	
	buffer[(*size)++] = 0x20; //ldc.i4
	*(int*)(&buffer[*size]) = type;
	*size += sizeof(type);
	buffer[(*size)++] = 0x20; //ldc.i4
	*(int*)(&buffer[*size]) = nameId;
	*size += sizeof(nameId);
	
	mdTypeDef controlType = 0;
	mdMethodDef getNameToken = 0;
	mdMethodDef getTextToken = 0;
	
	switch (type) {
		case 1: // Console
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
		case 2: // Windows Forms
			hr = metaData->FindTypeDefByName(L"System.Windows.Forms.Control", nullptr, &controlType);
			
			if (!SUCCEEDED(hr) || controlType == 0) {
				DebugWriteLine(L"FindTypeDefByName failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			hr = metaData->FindMethod(controlType, L"get_Name", getNameSig, sizeof(getNameSig), &getNameToken);
			
			if (!SUCCEEDED(hr) || getNameToken == 0) {
				DebugWriteLine(L"FindMethod failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			hr = metaData->FindMethod(controlType, L"get_Name", getNameSig, sizeof(getNameSig), &getNameToken);
			
			if (!SUCCEEDED(hr) || getNameToken == 0) {
				DebugWriteLine(L"FindMethod failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			hr = metaData->FindMethod(controlType, L"get_Text", getNameSig, sizeof(getNameSig), &getTextToken);
			
			if (!SUCCEEDED(hr) || getTextToken == 0) {
				DebugWriteLine(L"FindMethod failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			buffer[(*size)++] = 0x02; //ldarg.0
			buffer[(*size)++] = 0x28; //call
			*(mdMethodDef*)(&buffer[*size]) = getNameToken;
			*size += sizeof(getNameToken);

			buffer[(*size)++] = 0x02; //ldarg.0
			buffer[(*size)++] = 0x6F; //callvirt
			*(mdMethodDef*)(&buffer[*size]) = getTextToken;
			*size += sizeof(getTextToken);
			
			break;
		case 3: // WPF
			buffer[(*size)++] = 0x14; //ldnull
			buffer[(*size)++] = 0x14; //ldnull
			break;
	}
	
	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = loggerCall;
	*size += sizeof(loggerCall);

	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = activateCall;
	*size += sizeof(activateCall);
}

void CProfiler::ConvertToFat(byte *target, int *size)
{
	COR_ILMETHOD_FAT *targetTable =  (COR_ILMETHOD_FAT *)target;
    memset(targetTable, 0, FAT_HEADER_SIZE);
    targetTable->Flags = CorILMethod_FatFormat;
    targetTable->Size = FAT_HEADER_SIZE / sizeof(DWORD);
    targetTable->MaxStack = DEFAULT_MAX_STACK;
    *size = FAT_HEADER_SIZE;
}

/// <summary>
/// Moves all offsets in SEH section tables by the given count of bytes.
/// </summary>
void CProfiler::FixSEHSections(const COR_ILMETHOD_SECT *sections, int offset)
{
    while (sections) {
        if (sections->Kind() == CorILMethod_Sect_EHTable) {
            COR_ILMETHOD_SECT_EH *peh = (COR_ILMETHOD_SECT_EH *) sections;
			if (peh->IsFat()) {
                COR_ILMETHOD_SECT_EH_FAT *pFat = (COR_ILMETHOD_SECT_EH_FAT*) peh;

                for (unsigned int i = 0;  i < peh->EHCount();  i++) {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT *pfeh = &pFat->Clauses[i];

                    if (pfeh->Flags & COR_ILEXCEPTION_CLAUSE_FILTER)
                        pfeh->FilterOffset += offset;

                    pfeh->HandlerOffset += offset;
                    pfeh->TryOffset += offset;
                }
            } else {
                COR_ILMETHOD_SECT_EH_SMALL *pSmall = (COR_ILMETHOD_SECT_EH_SMALL *) peh;

                for (unsigned int i = 0;  i < peh->EHCount();  i++) {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL *pseh = &pSmall->Clauses[i];

                    if (pseh->Flags & COR_ILEXCEPTION_CLAUSE_FILTER)
                        pseh->FilterOffset += offset;

                    pseh->TryOffset += offset;
                    pseh->HandlerOffset += offset;
                }
            }
        }
        sections = sections->Next();
    } 
}

bool CProfiler::CreateMethod(IMetaDataEmit *emitter, const WCHAR *name, PCCOR_SIGNATURE sig, mdMethodDef *methodDefinition, mdToken moduleRef)
{
	HRESULT hr = emitter->DefineMethod(mdTokenNil, name, mdPublic | mdStatic | mdPinvokeImpl, sig, sizeof(sig), 0, miIL | miManaged, methodDefinition);
	
	if (!SUCCEEDED(hr))
		return false;
	
	hr = emitter->DefinePinvokeMap(*methodDefinition, pmNoMangle | pmCallConvStdcall, name, moduleRef);
	
	if (!SUCCEEDED(hr))
		return false;
	
	return true;
}

STDMETHODIMP CProfiler::ModuleLoadFinished(ModuleID moduleID, HRESULT /*hrStatus*/)
{
	HRESULT hr = S_OK;
	
	IMetaDataEmit *pIMetaDataEmit = nullptr;
	IMetaDataAssemblyImport *asmMetaData = nullptr;
	
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead | ofWrite, IID_IMetaDataEmit, (LPUNKNOWN *) &pIMetaDataEmit);
		
	if (!SUCCEEDED(hr) || pIMetaDataEmit == nullptr)
		goto CLEANUP;
	
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead | ofWrite, IID_IMetaDataAssemblyImport, (LPUNKNOWN *) &asmMetaData);

	if (!SUCCEEDED(hr) || asmMetaData == nullptr)
		goto CLEANUP;
		
	mdToken moduleRef;
	hr = pIMetaDataEmit->DefineModuleRef(HOOK_MODULE_NAME, &moduleRef);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
		
	mdMethodDef deactivator;
	if (!CreateMethod(pIMetaDataEmit, L"DeactivateProfiler", rgSig, &deactivator, moduleRef))
		goto CLEANUP;
		
	mdMethodDef activator;
	if (!CreateMethod(pIMetaDataEmit, L"ActivateProfiler", rgSig, &activator, moduleRef))
		goto CLEANUP;
		
	mdMethodDef logger;
	hr = pIMetaDataEmit->DefineMethod(mdTokenNil, L"LogEvent", mdPublic | mdStatic | mdPinvokeImpl, loggerSig, sizeof(loggerSig), 0, miIL | miManaged, &logger);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
		
	hr = pIMetaDataEmit->DefinePinvokeMap(logger, pmNoMangle | pmCallConvStdcall | pmCharSetUnicode, L"LogEvent", moduleRef);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
	
	{
		methodMapCriticalSection.Enter();
		TMethodTokenMap::iterator it = this->methodMap.find(moduleID);
		if (it == this->methodMap.end()) {
			this->methodMap.insert(TMethodTokenPair(moduleID, HandlerInfo(activator, deactivator, logger)));
		}
		methodMapCriticalSection.Leave();
	}
	
CLEANUP:
	if (pIMetaDataEmit != nullptr)
		pIMetaDataEmit->Release();
	
	if (asmMetaData != nullptr)
		asmMetaData->Release();
	
	return S_OK;
}