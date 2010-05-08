/*****************************************************************************
 * DotNetProfiler
 * 
 * Copyright (c) 2006 Scott Hackett
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author be held liable for any damages arising from the
 * use of this software. Permission to use, copy, modify, distribute and sell
 * this software for any purpose is hereby granted without fee, provided that
 * the above copyright notice appear in all copies and that both that
 * copyright notice and this permission notice appear in supporting
 * documentation.
 * 
 * Scott Hackett (code@scotthackett.com)
 *****************************************************************************/

#include "main.h"
#include "CorProfilerCallbackImpl.h"

// Disable "unreferenced formal parameter" warning
#pragma warning( disable : 4100 )

CCorProfilerCallbackImpl::CCorProfilerCallbackImpl() {}

STDMETHODIMP CCorProfilerCallbackImpl::Initialize(IUnknown *pICorProfilerInfoUnk)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::Shutdown()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AppDomainCreationStarted(AppDomainID appDomainID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AppDomainCreationFinished(AppDomainID appDomainID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AppDomainShutdownStarted(AppDomainID appDomainID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AppDomainShutdownFinished(AppDomainID appDomainID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AssemblyLoadStarted(AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AssemblyLoadFinished(AssemblyID assemblyID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AssemblyUnloadStarted(AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::AssemblyUnloadFinished(AssemblyID assemblyID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ModuleLoadStarted(ModuleID moduleID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ModuleLoadFinished(ModuleID moduleID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ModuleUnloadStarted(ModuleID moduleID)
{
    return S_OK;
}
	  
STDMETHODIMP CCorProfilerCallbackImpl::ModuleUnloadFinished(ModuleID moduleID, HRESULT hrStatus)
{
	return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ModuleAttachedToAssembly(ModuleID moduleID, AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ClassLoadStarted(ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ClassLoadFinished(ClassID classID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ClassUnloadStarted(ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ClassUnloadFinished(ClassID classID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::FunctionUnloadStarted(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITCompilationStarted(FunctionID functionID, BOOL fIsSafeToBlock)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITCompilationFinished(FunctionID functionID, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITCachedFunctionSearchStarted(FunctionID functionID, BOOL *pbUseCachedFunction)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITCachedFunctionSearchFinished(FunctionID functionID, COR_PRF_JIT_CACHE result)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITFunctionPitched(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::JITInlining(FunctionID callerID, FunctionID calleeID, BOOL *pfShouldInline)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::UnmanagedToManagedTransition(FunctionID functionID, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ManagedToUnmanagedTransition(FunctionID functionID, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ThreadCreated(ThreadID threadID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ThreadDestroyed(ThreadID threadID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ThreadAssignedToOSThread(ThreadID managedThreadID, DWORD osThreadID) 
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingClientInvocationStarted()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingClientSendingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingClientReceivingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingClientInvocationFinished()
{
	return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingServerReceivingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingServerInvocationStarted()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingServerInvocationReturned()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RemotingServerSendingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeSuspendFinished()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeSuspendAborted()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeResumeStarted()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeResumeFinished()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeThreadSuspended(ThreadID threadID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RuntimeThreadResumed(ThreadID threadID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::MovedReferences(ULONG cmovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ObjectAllocated(ObjectID objectID, ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ObjectsAllocatedByClass(ULONG classCount, ClassID classIDs[], ULONG objects[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ObjectReferences(ObjectID objectID, ClassID classID, ULONG objectRefs, ObjectID objectRefIDs[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RootReferences(ULONG rootRefs, ObjectID rootRefIDs[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionThrown(ObjectID thrownObjectID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionUnwindFunctionEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionUnwindFunctionLeave()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionSearchFunctionEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionSearchFunctionLeave()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionSearchFilterEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionSearchFilterLeave()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionSearchCatcherFound(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionCLRCatcherFound()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionCLRCatcherExecute()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionOSHandlerEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionOSHandlerLeave(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionUnwindFinallyEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionUnwindFinallyLeave()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionCatcherEnter(FunctionID functionID,
    											 ObjectID objectID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ExceptionCatcherLeave()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::COMClassicVTableCreated(ClassID wrappedClassID, REFGUID implementedIID, void *pVTable, ULONG cSlots)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::COMClassicVTableDestroyed(ClassID wrappedClassID, REFGUID implementedIID, void *pVTable)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::ThreadNameChanged(ThreadID threadID, ULONG cchName, WCHAR name[])
{
	return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
{
	return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::GarbageCollectionFinished()
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::RootReferences2(ULONG cRootRefs, ObjectID rootRefIDs[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIDs[])
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::HandleCreated(GCHandleID handleID, ObjectID initialObjectID)
{
    return S_OK;
}

STDMETHODIMP CCorProfilerCallbackImpl::HandleDestroyed(GCHandleID handleID)
{
    return S_OK;
}



