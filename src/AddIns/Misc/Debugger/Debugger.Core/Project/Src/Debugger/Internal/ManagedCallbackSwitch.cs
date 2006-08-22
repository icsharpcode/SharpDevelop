// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// Regular expresion:
// ^{\t*}{(:Ll| )*{:i} *\(((.# {:i}, |\))|())^6\)*}\n\t*\{(.|\n)@^\1\}
// Output: \1 - intention   \2 - declaration \3 - function name  \4-9 parameters

// Replace with:
// \1\2\n\1{\n\1\tGetProcessCallbackInterface(\4).\3(\4, \5, \6, \7, \8, \9);\n\1}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// This class forwards the callback the the approprite process
	/// </summary>
	class ManagedCallbackSwitch
	{
		NDebugger debugger;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public ManagedCallbackSwitch(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		public ManagedCallback GetProcessCallbackInterface(ICorDebugController c)
		{
			if (c.Is<ICorDebugAppDomain>()) {
				return GetProcessCallbackInterface(c.CastTo<ICorDebugAppDomain>());
			} else if (c.Is<ICorDebugProcess>()){
				return GetProcessCallbackInterface(c.CastTo<ICorDebugProcess>());
			} else {
				throw new System.Exception("Unknown callback argument");
			}
		}
		
		public ManagedCallback GetProcessCallbackInterface(ICorDebugAppDomain pAppDomain)
		{
			return GetProcessCallbackInterface(pAppDomain.Process);
		}
		
		public ManagedCallback GetProcessCallbackInterface(ICorDebugProcess pProcess)
		{
			Process process = debugger.GetProcess(pProcess);
			return process.CallbackInterface;
		}
		
		#region Program folow control
		
		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			GetProcessCallbackInterface(pAppDomain).StepComplete(pAppDomain, pThread, pStepper, reason);
		}
		
		// Do not pass the pBreakpoint parameter as ICorDebugBreakpoint - marshaling of it fails in .NET 1.1
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			GetProcessCallbackInterface(pAppDomain).Breakpoint(pAppDomain, pThread, pBreakpoint);
		}
		
		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
		{
			GetProcessCallbackInterface(pAppDomain).BreakpointSetError(pAppDomain, pThread, pBreakpoint, dwError);
		}
		
		public unsafe void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			GetProcessCallbackInterface(pAppDomain).Break(pAppDomain, pThread);
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			GetProcessCallbackInterface(pProcess).ControlCTrap(pProcess);
		}

		public unsafe void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
		{
			GetProcessCallbackInterface(pAppDomain).Exception(pAppDomain, pThread, unhandled);
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			GetProcessCallbackInterface(pAppDomain).LogSwitch(pAppDomain, pThread, lLevel, ulReason, pLogSwitchName, pParentName);
		}
		
		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			GetProcessCallbackInterface(pAppDomain).LogMessage(pAppDomain, pThread, lLevel, pLogSwitchName, pMessage);
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
		{
			GetProcessCallbackInterface(pAppDomain).EditAndContinueRemap(pAppDomain, pThread, pFunction, fAccurate);
		}
		
		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			GetProcessCallbackInterface(pAppDomain).EvalException(pAppDomain, pThread, corEval);
		}
		
		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			GetProcessCallbackInterface(pAppDomain).EvalComplete(pAppDomain, pThread, corEval);
		}
		
		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			GetProcessCallbackInterface(pProcess).DebuggerError(pProcess, errorHR, errorCode);
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, IStream pSymbolStream)
		{
			GetProcessCallbackInterface(pAppDomain).UpdateModuleSymbols(pAppDomain, pModule, pSymbolStream);
		}

		#endregion

		#region Start of Application

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			GetProcessCallbackInterface(pProcess).CreateProcess(pProcess);
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			GetProcessCallbackInterface(pProcess).CreateAppDomain(pProcess, pAppDomain);
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			GetProcessCallbackInterface(pAppDomain).LoadAssembly(pAppDomain, pAssembly);
		}

		public unsafe void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			GetProcessCallbackInterface(pAppDomain).LoadModule(pAppDomain, pModule);
		}

		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			if (pAppDomain != null) {
				GetProcessCallbackInterface(pAppDomain).NameChange(pAppDomain, pThread);
			}
			if (pThread != null) {
				GetProcessCallbackInterface(pThread.Process).NameChange(pAppDomain, pThread);
			}
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			GetProcessCallbackInterface(pAppDomain).CreateThread(pAppDomain, pThread);
		}

		public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			GetProcessCallbackInterface(pAppDomain).LoadClass(pAppDomain, c);
		}

		#endregion

		#region Exit of Application

		public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			GetProcessCallbackInterface(pAppDomain).UnloadClass(pAppDomain, c);
		}

		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			GetProcessCallbackInterface(pAppDomain).UnloadModule(pAppDomain, pModule);
		}

		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			GetProcessCallbackInterface(pAppDomain).UnloadAssembly(pAppDomain, pAssembly);
		}

		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			GetProcessCallbackInterface(pAppDomain).ExitThread(pAppDomain, pThread);
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			GetProcessCallbackInterface(pProcess).ExitAppDomain(pProcess, pAppDomain);
		}
		
		public void ExitProcess(ICorDebugProcess pProcess)
		{
			GetProcessCallbackInterface(pProcess).ExitProcess(pProcess);
		}
		
		#endregion
		
		#region ICorDebugManagedCallback2 Members
		
		public void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			GetProcessCallbackInterface(pProcess).ChangeConnection(pProcess, dwConnectionId);
		}

		public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, IntPtr pConnName)
		{
			GetProcessCallbackInterface(pProcess).CreateConnection(pProcess, dwConnectionId, pConnName);
		}

		public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			GetProcessCallbackInterface(pProcess).DestroyConnection(pProcess, dwConnectionId);
		}

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType exceptionType, uint dwFlags)
		{
			GetProcessCallbackInterface(pAppDomain).Exception2(pAppDomain, pThread, pFrame, nOffset, exceptionType, dwFlags);
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			GetProcessCallbackInterface(pAppDomain).ExceptionUnwind(pAppDomain, pThread, dwEventType, dwFlags);
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
		{
			GetProcessCallbackInterface(pAppDomain).FunctionRemapComplete(pAppDomain, pThread, pFunction);
		}

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
		{
			GetProcessCallbackInterface(pAppDomain).FunctionRemapOpportunity(pAppDomain, pThread, pOldFunction, pNewFunction, oldILOffset);
		}

		public void MDANotification(ICorDebugController c, ICorDebugThread t, ICorDebugMDA mda)
		{
			GetProcessCallbackInterface(c).MDANotification(c, t, mda);
		}

		#endregion
	}
}
