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

#include "main.h"
#include "global.h"
#include "Profiler.h"
#include "ProfilerFactory.h"
#include "CriticalSection.h"

#include <initguid.h>
#include "LightweightList.h"

long g_cRefThisDll;
HANDLE g_module;
LightweightList *allThreadLocalDatas;
DWORD tls_index;

ThreadLocalData *AttachToThread()
{
	ThreadLocalData *data = allThreadLocalDatas->add();
	TlsSetValue(tls_index, data);
	return data;
}

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID //lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH: 
			DebugWriteLine(L"DLL_PROCESS_ATTACH");
			g_module = hModule;
			tls_index = TlsAlloc();
			TlsSetValue(tls_index, nullptr);
			//AttachToThread();
			break;

		case DLL_THREAD_ATTACH: {
			DebugWriteLine(L"DLL_THREAD_ATTACH");
			TlsSetValue(tls_index, nullptr);
			//AttachToThread();
			break;
		}

		case DLL_THREAD_DETACH: {
			DebugWriteLine(L"DLL_THREAD_DETACH");
			ThreadLocalData *data = getThreadLocalData();
			DetachFromThread(data);
			if (data != nullptr)
				allThreadLocalDatas->remove(data);
			break;
		}
		
		case DLL_PROCESS_DETACH: {
			DebugWriteLine(L"DLL_PROCESS_DETACH");
			break;
		}
	}

	return TRUE;
}

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID *ppvOut)
{
	// MessageBox(nullptr, TEXT("DllGetClassObject"), TEXT("DllGetClassObject"), MB_OK);

	*ppvOut = nullptr;
    if (IsEqualIID(rclsid, CLSID_Profiler))
    {
       // declare a classfactory for CProfiler class 
       CProfilerFactory *pcf = new CProfilerFactory;
       return pcf->QueryInterface(riid,ppvOut);
    }
    return CLASS_E_CLASSNOTAVAILABLE;
}

STDAPI  DllCanUnloadNow(void)
{
    return (g_cRefThisDll == 0 ? S_OK : S_FALSE);
}
