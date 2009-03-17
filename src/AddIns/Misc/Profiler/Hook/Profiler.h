// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#pragma once
#include "CorProfilerCallbackImpl.h"
#include <rpc.h>
#include <rpcndr.h>

#include "ProfilerMetaData.h"

#include "SharedMemory.h"
#include "CircularBuffer.h"
#include "CriticalSection.h"

#include <map>

typedef IID CLSID;
#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

MIDL_DEFINE_GUID(CLSID, CLSID_Profiler,0xE7E2C111,0x3471,0x4AC7,0xB2,0x78,0x11,0xF4,0xC2,0x6E,0xDB,0xCF);

class ThreadInfo;

struct ThreadLocalData;

typedef std::map<FunctionID, int> TFunctionIDMap;
typedef std::pair<FunctionID, int> TFunctionIDPair;

typedef std::map<ThreadID, ThreadInfo*> TThreadIDMap;
typedef std::pair<ThreadID, ThreadInfo*> TThreadIDPair;

typedef std::map<DWORD, ThreadInfo*> TUThreadIDMap;
typedef std::pair<DWORD, ThreadInfo*> TUThreadIDPair;

// CProfiler
class CProfiler : public CCorProfilerCallbackImpl
{
public:
	STDMETHOD(QueryInterface)(REFIID riid,LPVOID *ppv);
	STDMETHOD_(ULONG,AddRef)() ;
	STDMETHOD_(ULONG,Release)() ;

    // STARTUP/SHUTDOWN EVENTS
    STDMETHOD(Initialize)(IUnknown *pICorProfilerInfoUnk);
    STDMETHOD(Shutdown)();
    
    // THREAD EVENTS
    STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadID, DWORD osThreadID);
    STDMETHOD(ThreadNameChanged)(ThreadID threadID, ULONG cchName, WCHAR name[]);
    STDMETHOD(ThreadCreated)(ThreadID threadID);
    STDMETHOD(ThreadDestroyed)(ThreadID threadID);

	// EXCEPTION EVENTS
	// ECMA-335, page 82 (12.4.2 Exception handling)
	// ECMA-335, page 96 (Partition II, 19 Exception handling)

    // Exception creation
    STDMETHOD(ExceptionThrown)(ObjectID thrownObjectID);
    // Search phase
    STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionID);
    STDMETHOD(ExceptionSearchFunctionLeave)();
    STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionID);
    STDMETHOD(ExceptionSearchFilterLeave)();
    STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionID);
    STDMETHOD(ExceptionCLRCatcherFound)();
    STDMETHOD(ExceptionCLRCatcherExecute)();
    STDMETHOD(ExceptionOSHandlerEnter)(FunctionID functionID);
    STDMETHOD(ExceptionOSHandlerLeave)(FunctionID functionID);
    // Unwind phase
    STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionID);
    STDMETHOD(ExceptionUnwindFunctionLeave)();
    STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionID);
    STDMETHOD(ExceptionUnwindFinallyLeave)();
    STDMETHOD(ExceptionCatcherEnter)(FunctionID functionID, ObjectID objectID);
    STDMETHOD(ExceptionCatcherLeave)();
    
    // UNLOAD EVENTS
    STDMETHOD(FunctionUnloadStarted)(FunctionID functionID);

	CProfiler();

	// mapping functions
	static UINT_PTR _stdcall FunctionMapper(FunctionID functionId, BOOL *pbHookFunction);
	UINT_PTR MapFunction(FunctionID functionID);
	FunctionInfo *CreateNewRoot();
	void MovedRootChild(FunctionInfo *newRootChild);
	void CProfiler::EnterLock(ThreadLocalData *);

	// logging function
    void LogString(WCHAR* pszFmtString, ... );
private:
    // container for ICorProfilerInfo reference
	ICorProfilerInfo  *pICorProfilerInfo;
    // container for ICorProfilerInfo2 reference
	ICorProfilerInfo2 *pICorProfilerInfo2;
	// variables needed for interprocess communication
	CSharedMemory *sharedMemory;
	CCircularBuffer *nativeToManagedBuffer;
	CriticalSection nativeToManagedCriticalSection;
	CriticalSection rootElementCriticalSection;
	CriticalSection mapFunctionCriticalSection;
	CriticalSection threadMapCriticalSection;
	CEventWaitHandle *accessEventHandle;

	SignatureReader *sigReader;
	TFunctionIDMap functionIDMap;
	TThreadIDMap threadIDMap;
	TUThreadIDMap unmanagedThreadIDMap;
		
	// function to set up our event mask
	HRESULT SetEventMask();
	int InitializeCommunication();
};

DWORD WINAPI ExecuteHostCommand( LPVOID lpParam );
void DetachFromThread(ThreadLocalData *data);
