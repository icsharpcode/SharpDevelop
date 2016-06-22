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