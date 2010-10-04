// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma once

#include "global.h"
#include "stdio.h"
#include "Profiler.h"

class CProfilerFactory : IClassFactory
{
public:
	STDMETHODIMP_(ULONG) AddRef() 
	{
		return 1; // Singleton
	}

	STDMETHODIMP_(ULONG) Release() 
	{
		return 1; // Singleton
	}

	STDMETHOD(QueryInterface)(REFIID riid,LPVOID *ppv)
	{
		*ppv = nullptr;
		if(IsEqualIID(riid,IID_IUnknown) || IsEqualIID(riid,IID_IClassFactory)) {
			*ppv = this;
			return S_OK;
		}
		return E_NOINTERFACE;
	}

	STDMETHODIMP CreateInstance(LPUNKNOWN pUnkOuter, REFIID riid, LPVOID *ppvObj)
	{
		*ppvObj = nullptr;
		if (pUnkOuter)
    		return CLASS_E_NOAGGREGATION;
		HRESULT hr = profiler.QueryInterface(riid, ppvObj);
		return hr;
	}

	STDMETHODIMP LockServer(BOOL) {	return S_OK; }  // not implemented
};