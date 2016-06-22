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

#include "CorProfilerCallbackImpl.h"
#include <rpc.h>
#include <rpcndr.h>

#include "ProfilerMetaData.h"

#include "SharedMemory.h"
#include "CircularBuffer.h"
#include "CriticalSection.h"

// Disable: conditional expression is constant (C4127)
#pragma warning (disable: 4127)
#include "corhlpr.h"
#pragma warning (default: 4127)

#include <map>

typedef IID CLSID;
#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

MIDL_DEFINE_GUID(CLSID, CLSID_Profiler,0xE7E2C111,0x3471,0x4AC7,0xB2,0x78,0x11,0xF4,0xC2,0x6E,0xDB,0xCF);

class ThreadInfo;

class HandlerInfo;

struct ThreadLocalData;

typedef std::map<FunctionID, int> TFunctionIDMap;
typedef std::pair<FunctionID, int> TFunctionIDPair;

typedef std::map<ThreadID, ThreadInfo*> TThreadIDMap;
typedef std::pair<ThreadID, ThreadInfo*> TThreadIDPair;

typedef std::map<DWORD, ThreadInfo*> TUThreadIDMap;
typedef std::pair<DWORD, ThreadInfo*> TUThreadIDPair;

typedef std::map<ModuleID, HandlerInfo> TMethodTokenMap;
typedef std::pair<ModuleID, HandlerInfo> TMethodTokenPair;

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
//    STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionID);
//    STDMETHOD(ExceptionSearchFunctionLeave)();
//    STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionID);
//    STDMETHOD(ExceptionSearchFilterLeave)();
//    STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionID);
//    STDMETHOD(ExceptionCLRCatcherFound)();
//    STDMETHOD(ExceptionCLRCatcherExecute)();
//    STDMETHOD(ExceptionOSHandlerEnter)(FunctionID functionID);
//    STDMETHOD(ExceptionOSHandlerLeave)(FunctionID functionID);
    // Unwind phase
//    STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionID);
    STDMETHOD(ExceptionUnwindFunctionLeave)();
//    STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionID);
//    STDMETHOD(ExceptionUnwindFinallyLeave)();
//    STDMETHOD(ExceptionCatcherEnter)(FunctionID functionID, ObjectID objectID);
//    STDMETHOD(ExceptionCatcherLeave)();
    
    // UNLOAD EVENTS
    STDMETHOD(FunctionUnloadStarted)(FunctionID functionID);
    
    // JITTER EVENTS
    STDMETHOD(JITCompilationStarted)(FunctionID functionID, BOOL fIsSafeToBlock);
    
 	// MODULE EVENTS
	STDMETHOD(ModuleLoadFinished)(ModuleID moduleID, HRESULT hrStatus);

	CProfiler();

	// mapping functions
	static UINT_PTR _stdcall FunctionMapper(FunctionID functionId, BOOL *pbHookFunction);
	UINT_PTR MapFunction(FunctionID functionID, const WCHAR **name);
	FunctionInfo *CreateNewRoot();
	void MovedRootChild(FunctionInfo *newRootChild);
	void EnterLock(ThreadLocalData *);
	void Deactivate();
	void Activate();

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
	CriticalSection methodMapCriticalSection;
	CEventWaitHandle *accessEventHandle;

	SignatureReader *sigReader;
	TFunctionIDMap functionIDMap;
	TThreadIDMap threadIDMap;
	TUThreadIDMap unmanagedThreadIDMap;
	TMethodTokenMap methodMap;
	
	// function to set up our event mask
	HRESULT SetEventMask();
	void Rewrite(FunctionID functionID, int type, int nameId);
	void ConvertToFat(byte *target, int *size);
	void FixSEHSections(const COR_ILMETHOD_SECT *sections, int offset);
	int SetInjectionCode(IMetaDataImport *, byte *buffer, int *size, mdMethodDef activateCall, mdMethodDef loggerCall, mdMethodDef deactivateCall, int type, int nameId);
	bool CreateMethod(IMetaDataEmit *, const WCHAR *, PCCOR_SIGNATURE, mdMethodDef *, mdToken);
	int InitializeCommunication();
};

void DetachFromThread(ThreadLocalData *data);