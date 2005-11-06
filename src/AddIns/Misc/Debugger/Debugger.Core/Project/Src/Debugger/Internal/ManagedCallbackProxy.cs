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

using DebuggerInterop.Core;

// Regular expresion:
// ^{\t*}{(:Ll| )*{:i} *\(((.# {:i}, |\))|())^6\)*}\n\t*\{(.|\n)@\}
// Output: \1 - intention   \2 - declaration \3 - function name  \4-9 parameters

// Replace with:
// \1\2\n\1{\n\1\tCallbackReceived("\3", new object[] {\4, \5, \6, \7, \8, \9});\n\1}

namespace Debugger
{
	class ManagedCallbackProxy :ICorDebugManagedCallback, ICorDebugManagedCallback2
	{
		NDebugger debugger;
		ManagedCallback realCallback;
		MTA2STA mta2sta;
		
		public ManagedCallbackProxy(NDebugger debugger, ManagedCallback realCallback)
		{
			this.debugger = debugger;
			this.realCallback = realCallback;
			mta2sta = new MTA2STA();
		}
		
		private void CallbackReceived (string function, object[] parameters)
		{
			if (debugger.RequiredApartmentState == ApartmentState.STA) {
				mta2sta.CallInSTA(realCallback, function, parameters);
			} else {
				mta2sta.Call(realCallback, function, parameters);
			}
		}
		
		

		public void StepComplete(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pStepper, DebuggerInterop.Core.CorDebugStepReason reason)
		{
			CallbackReceived("StepComplete", new object[] {pAppDomain, pThread, pStepper, reason});
		}

		public void Break(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			CallbackReceived("Break", new object[] {pAppDomain, pThread});
		}

		public void ControlCTrap(System.IntPtr pProcess)
		{
			CallbackReceived("ControlCTrap", new object[] {pProcess});
		}

		public void Exception(System.IntPtr pAppDomain, System.IntPtr pThread, int unhandled)
		{
			CallbackReceived("Exception", new object[] {pAppDomain, pThread, unhandled});
		}

		public void Breakpoint(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pBreakpoint)
		{
			CallbackReceived("Breakpoint", new object[] {pAppDomain, pThread, pBreakpoint});
		}

		public void CreateProcess(System.IntPtr pProcess)
		{
			CallbackReceived("CreateProcess", new object[] {pProcess});
		}

		public void CreateAppDomain(System.IntPtr pProcess, System.IntPtr pAppDomain)
		{
			CallbackReceived("CreateAppDomain", new object[] {pProcess, pAppDomain});
		}

		public void CreateThread(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			CallbackReceived("CreateThread", new object[] {pAppDomain, pThread});
		}

		public void LoadAssembly(System.IntPtr pAppDomain, System.IntPtr pAssembly)
		{
			CallbackReceived("LoadAssembly", new object[] {pAppDomain, pAssembly});
		}

		public void LoadModule(System.IntPtr pAppDomain, System.IntPtr pModule)
		{
			CallbackReceived("LoadModule", new object[] {pAppDomain, pModule});
		}

		public void NameChange(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			CallbackReceived("NameChange", new object[] {pAppDomain, pThread});
		}

		public void LoadClass(System.IntPtr pAppDomain, System.IntPtr c)
		{
			CallbackReceived("LoadClass", new object[] {pAppDomain, c});
		}

		public void UnloadClass(System.IntPtr pAppDomain, System.IntPtr c)
		{
			CallbackReceived("UnloadClass", new object[] {pAppDomain, c});
		}

		public void ExitThread(System.IntPtr pAppDomain, System.IntPtr pThread)
		{
			CallbackReceived("ExitThread", new object[] {pAppDomain, pThread});
		}

		public void UnloadModule(System.IntPtr pAppDomain, System.IntPtr pModule)
		{
			CallbackReceived("UnloadModule", new object[] {pAppDomain, pModule});
		}

		public void UnloadAssembly(System.IntPtr pAppDomain, System.IntPtr pAssembly)
		{
			CallbackReceived("UnloadAssembly", new object[] {pAppDomain, pAssembly});
		}

		public void ExitAppDomain(System.IntPtr pProcess, System.IntPtr pAppDomain)
		{
			CallbackReceived("ExitAppDomain", new object[] {pProcess, pAppDomain});
		}

		public void ExitProcess(System.IntPtr pProcess)
		{
			CallbackReceived("ExitProcess", new object[] {pProcess});
		}

		public void BreakpointSetError(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pBreakpoint, uint dwError)
		{
			CallbackReceived("BreakpointSetError", new object[] {pAppDomain, pThread, pBreakpoint, dwError});
		}

		public void LogSwitch(System.IntPtr pAppDomain, System.IntPtr pThread, int lLevel, uint ulReason, System.IntPtr pLogSwitchName, System.IntPtr pParentName)
		{
			CallbackReceived("LogSwitch", new object[] {pAppDomain, pThread, lLevel, ulReason, pLogSwitchName});
		}

		public void EvalException(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pEval)
		{
			CallbackReceived("EvalException", new object[] {pAppDomain, pThread, pEval});
		}

		public void LogMessage(System.IntPtr pAppDomain, System.IntPtr pThread, int lLevel, System.IntPtr pLogSwitchName, System.IntPtr pMessage)
		{
			CallbackReceived("LogMessage", new object[] {pAppDomain, pThread, lLevel, pLogSwitchName, pMessage});
		}

		public void EditAndContinueRemap(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pFunction, int fAccurate)
		{
			CallbackReceived("EditAndContinueRemap", new object[] {pAppDomain, pThread, pFunction, fAccurate});
		}

		public void EvalComplete(System.IntPtr pAppDomain, System.IntPtr pThread, System.IntPtr pEval)
		{
			CallbackReceived("EvalComplete", new object[] {pAppDomain, pThread, pEval});
		}

		public void DebuggerError(System.IntPtr pProcess, int errorHR, uint errorCode)
		{
			CallbackReceived("DebuggerError", new object[] {pProcess, errorHR, errorCode});
		}

		public void UpdateModuleSymbols(System.IntPtr pAppDomain, System.IntPtr pModule, System.IntPtr pSymbolStream)
		{
			CallbackReceived("UpdateModuleSymbols", new object[] {pAppDomain, pModule, pSymbolStream});
		}



		#region ICorDebugManagedCallback2 Members

		public void ChangeConnection(IntPtr pProcess, uint dwConnectionId)
		{
			CallbackReceived("ChangeConnection", new object[] {pProcess, dwConnectionId});
		}

		public void CreateConnection(IntPtr pProcess, uint dwConnectionId, ref ushort pConnName)
		{
			CallbackReceived("CreateConnection", new object[] {pProcess, dwConnectionId, pConnName});
		}

		public void DestroyConnection(IntPtr pProcess, uint dwConnectionId)
		{
			CallbackReceived("DestroyConnection", new object[] {pProcess, dwConnectionId});
		}

		public void Exception(IntPtr pAppDomain, IntPtr pThread, IntPtr pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
		{
			CallbackReceived("Exception2", new object[] {pAppDomain, pThread, pFrame, nOffset, dwEventType, dwFlags});
		}

		public void ExceptionUnwind(IntPtr pAppDomain, IntPtr pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			CallbackReceived("ExceptionUnwind", new object[] {pAppDomain, pThread, dwEventType, dwFlags});
		}

		public void FunctionRemapComplete(IntPtr pAppDomain, IntPtr pThread, IntPtr pFunction)
		{
			CallbackReceived("FunctionRemapComplete", new object[] {pAppDomain, pThread, pFunction});
		}

		public void FunctionRemapOpportunity(IntPtr pAppDomain, IntPtr pThread, IntPtr pOldFunction, IntPtr pNewFunction, uint oldILOffset)
		{
			CallbackReceived("FunctionRemapOpportunity", new object[] {pAppDomain, pThread, pOldFunction, pNewFunction, oldILOffset});
		}

		public void MDANotification(IntPtr pController, IntPtr pThread, IntPtr pMDA)
		{
			CallbackReceived("MDANotification", new object[] {pController, pThread, pMDA});
		}

		#endregion
	}

}
