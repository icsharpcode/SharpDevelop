// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Debugger.Interop.CorDebug;

// Regular expresion:
// ^{\t*}{(:Ll| )*{:i} *\(((.# {:i}, |\))|())^6\)*}\n\t*\{(.|\n)@\t\}
// Output: \1 - intention   \2 - declaration \3 - function name  \4-9 parameters

// Replace with:
// \1\2\n\1{\n\1\tCallbackReceived("\3", new object[] {\4, \5, \6, \7, \8, \9});\n\1}
// \1\2\n\1{\n\1\tCall(delegate {\n\1\t     \trealCallback.\3(\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\4),\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\5),\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\6),\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\7),\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\8),\n\1\t     \t\tMTA2STA.MarshalIntPtrTo(\9),\n\1\t     \t);\n\1\t     });\n\1}

namespace Debugger
{
	class ManagedCallbackProxy :ICorDebugManagedCallback, ICorDebugManagedCallback2
	{
		NDebugger debugger;
		ManagedCallback realCallback;
		MTA2STA mta2sta;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public ManagedCallbackProxy(ManagedCallback realCallback)
		{
			this.debugger = realCallback.Debugger;
			this.realCallback = realCallback;
			mta2sta = new MTA2STA();
		}
		
		private void CallbackReceived (string function, object[] parameters)
		{
			if (debugger.RequiredApartmentState == ApartmentState.STA) {
				mta2sta.CallInSTA(realCallback, function, parameters);
			} else {
				MTA2STA.InvokeMethod(realCallback, function, parameters);
			}
		}
		
		void Call(MethodInvoker d)
		{
			mta2sta.CallInSTA(d);
		}
			
		public void StepComplete(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pStepper, Debugger.Interop.CorDebug.CorDebugStepReason reason)
		{
			Call(delegate {
			     	realCallback.StepComplete(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugStepper>(pStepper),
			     		reason
			     	);
			     });
		}
		
		public void Break(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			Call(delegate {
			     	realCallback.Break(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread)
			     	);
			     });
		}
		
		public void ControlCTrap(System.IntPtr pProcess)
		{
			Call(delegate {
			     	realCallback.ControlCTrap(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess)
			     	);
			     });
		}
		
		public void Exception(System.IntPtr pAppDomain, System.IntPtr pThread, int unhandled)
		{
			Call(delegate {
			     	realCallback.Exception(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		unhandled
			     	);
			     });
		}
		
		public void Breakpoint(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pBreakpoint)
		{
			Call(delegate {
			     	realCallback.Breakpoint(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		pBreakpoint
			     	);
			     });
		}
		
		public void CreateProcess(System.IntPtr pProcess)
		{
			Call(delegate {
			     	realCallback.CreateProcess(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess)
			     	);
			     });
		}
		
		public void CreateAppDomain(System.IntPtr pProcess, System.IntPtr pAppDomain)
		{
			Call(delegate {
			     	realCallback.CreateAppDomain(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain)
			     	);
			     });
		}
		
		public void CreateThread(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			Call(delegate {
			     	realCallback.CreateThread(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread)
			     	);
			     });
		}
		
		public void LoadAssembly(System.IntPtr pAppDomain, System.IntPtr pAssembly)
		{
			Call(delegate {
			     	realCallback.LoadAssembly(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAssembly>(pAssembly)
			     	);
			     });
		}
		
		public void LoadModule(System.IntPtr pAppDomain, System.IntPtr pModule)
		{
			Call(delegate {
			     	realCallback.LoadModule(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugModule>(pModule)
			     	);
			     });
		}
		
		public void NameChange(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			Call(delegate {
			     	realCallback.NameChange(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread)
			     	);
			     });
		}
		
		public void LoadClass(System.IntPtr pAppDomain, System.IntPtr c)
		{
			Call(delegate {
			     	realCallback.LoadClass(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugClass>(c)
			     	);
			     });
		}
		
		public void UnloadClass(System.IntPtr pAppDomain, System.IntPtr c)
		{
			Call(delegate {
			     	realCallback.UnloadClass(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugClass>(c)
			     	);
			     });
		}
		
		public void ExitThread(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			Call(delegate {
			     	realCallback.ExitThread(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread)
			     	);
			     });
		}
		
		public void UnloadModule(System.IntPtr pAppDomain, System.IntPtr pModule)
		{
			Call(delegate {
			     	realCallback.UnloadModule(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugModule>(pModule)
			     	);
			     });
		}
		
		public void UnloadAssembly(System.IntPtr pAppDomain, System.IntPtr pAssembly)
		{
			Call(delegate {
			     	realCallback.UnloadAssembly(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAssembly>(pAssembly)
			     	);
			     });
		}
		
		public void ExitAppDomain(System.IntPtr pProcess, System.IntPtr pAppDomain)
		{
			Call(delegate {
			     	realCallback.ExitAppDomain(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain)
			     	);
			     });
		}
		
		public void ExitProcess(System.IntPtr pProcess)
		{
			Call(delegate {
			     	realCallback.ExitProcess(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess)
			     	);
			     });
		}
		
		public void BreakpointSetError(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pBreakpoint, uint dwError)
		{
			Call(delegate {
			     	realCallback.BreakpointSetError(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugBreakpoint>(pBreakpoint),
			     		dwError
			     	);
			     });
		}
		
		public void LogSwitch(System.IntPtr pAppDomain, System.IntPtr pThread, int lLevel, uint ulReason, System.IntPtr pLogSwitchName, System.IntPtr pParentName)
		{
			Call(delegate {
			     	realCallback.LogSwitch(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		lLevel,
			     		ulReason,
			     		MTA2STA.MarshalIntPtrTo<string>(pLogSwitchName),
			     		MTA2STA.MarshalIntPtrTo<string>(pParentName)
			     	);
			     });
		}
		
		public void EvalException(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pEval)
		{
			Call(delegate {
			     	realCallback.EvalException(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugEval>(pEval)
			     	);
			     });
		}
		
		public void LogMessage(System.IntPtr pAppDomain, System.IntPtr pThread, int lLevel, System.IntPtr pLogSwitchName, System.IntPtr pMessage)
		{
			Call(delegate {
			     	realCallback.LogMessage(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		lLevel,
			     		MTA2STA.MarshalIntPtrTo<string>(pLogSwitchName),
			     		MTA2STA.MarshalIntPtrTo<string>(pMessage)
			     	);
			     });
		}
		
		public void EditAndContinueRemap(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pFunction, int fAccurate)
		{
			Call(delegate {
			     	realCallback.EditAndContinueRemap(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugFunction>(pFunction),
			     		fAccurate
			     	);
			     });
		}
		
		public void EvalComplete(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pEval)
		{
			Call(delegate {
			     	realCallback.EvalComplete(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugEval>(pEval)
			     	);
			     });
		}
		
		public void DebuggerError(System.IntPtr pProcess, int errorHR, uint errorCode)
		{
			Call(delegate {
			     	realCallback.DebuggerError(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		errorHR,
			     		errorCode
			     	);
			     });
		}
		
		public void UpdateModuleSymbols(System.IntPtr pAppDomain, System.IntPtr pModule, System.IntPtr pSymbolStream)
		{
			Call(delegate {
			     	realCallback.UpdateModuleSymbols(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugModule>(pModule),
			     		MTA2STA.MarshalIntPtrTo<Debugger.Interop.CorDebug.IStream>(pSymbolStream)
			     	);
			     });
		}



		#region ICorDebugManagedCallback2 Members

		public void ChangeConnection(IntPtr pProcess, uint dwConnectionId)
		{
			Call(delegate {
			     	realCallback.ChangeConnection(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		dwConnectionId
			     	);
			     });
		}
		
		public void CreateConnection(IntPtr pProcess, uint dwConnectionId, ref ushort pConnName)
		{
			ushort pName = pConnName;
			Call(delegate {
			     	realCallback.CreateConnection(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		dwConnectionId,
			     		ref pName
			     	);
			     });
		}
		
		public void DestroyConnection(IntPtr pProcess, uint dwConnectionId)
		{
			Call(delegate {
			     	realCallback.DestroyConnection(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugProcess>(pProcess),
			     		dwConnectionId
			     	);
			     });
		}
		
		public void Exception(IntPtr pAppDomain, IntPtr pThread, IntPtr pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
		{
			Call(delegate {
			     	realCallback.Exception2(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugFrame>(pFrame),
			     		nOffset,
			     		dwEventType,
			     		dwFlags
			     	);
			     });
		}
		
		public void ExceptionUnwind(IntPtr pAppDomain, IntPtr pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			Call(delegate {
			     	realCallback.ExceptionUnwind(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		dwEventType,
			     		dwFlags
			     	);
			     });
		}
		
		public void FunctionRemapComplete(IntPtr pAppDomain, IntPtr pThread, IntPtr pFunction)
		{
			Call(delegate {
			     	realCallback.FunctionRemapComplete(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugFunction>(pFunction)
			     	);
			     });
		}
		
		public void FunctionRemapOpportunity(IntPtr pAppDomain, IntPtr pThread, IntPtr pOldFunction, IntPtr pNewFunction, uint oldILOffset)
		{
			Call(delegate {
			     	realCallback.FunctionRemapOpportunity(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugAppDomain>(pAppDomain),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugFunction>(pOldFunction),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugFunction>(pNewFunction),
			     		oldILOffset
			     	);
			     });
		}
		
		public void MDANotification(IntPtr pController, IntPtr pThread, IntPtr pMDA)
		{
			Call(delegate {
			     	realCallback.MDANotification(
			     		MTA2STA.MarshalIntPtrTo<ICorDebugController>(pController),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugThread>(pThread),
			     		MTA2STA.MarshalIntPtrTo<ICorDebugMDA>(pMDA)
			     	);
			     });
		}
		
		#endregion
	}

}
