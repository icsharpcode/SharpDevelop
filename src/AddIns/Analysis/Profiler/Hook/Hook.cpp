// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
