// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Tests the values of DebuggerService.CurrentDebugger.IsDebugging (debugger attached to a process)
	/// and DebuggerService.CurrentDebugger.IsProcessRunning (process is running and not in break mode).
	/// </summary>
	/// <attributes name="isdebugging">
	/// Optional; boolean value IsDebugging should have.
	/// </attributes>
	/// <attributes name="isprocessrunning">
	/// Optional; boolean value IsProcessRunning should have.
	/// </attributes>
	/// <example title="Test if currently no process is running">
	/// &lt;Condition name = "IsProcessRunning" isdebugging="False"&gt;
	/// </example>
	/// <example title="Test if the debugger is attached to anything">
	/// &lt;Condition name = "IsProcessRunning" isdebugging="True"&gt;
	/// </example>
	/// <example title="Test if the debugger is attached and we are in break mode">
	/// &lt;Condition name = "IsProcessRunning" isdebugging="True" isprocessrunning="False"&gt;
	/// </example>
	/// <example title="Test if the debugger is attached and the process is running">
	/// &lt;Condition name = "IsProcessRunning" isdebugging="True" isprocessrunning="True"&gt;
	/// </example>
	public class IsProcessRunningConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string isdebugging = condition.Properties.Get("isdebugging", String.Empty);
			string isprocessrunning = condition.Properties.Get("isprocessrunning", String.Empty);
			bool debuggerIsDebugging = DebuggerService.IsDebuggerLoaded ? DebuggerService.CurrentDebugger.IsDebugging : false;
			bool debuggerIsProcessRunning = DebuggerService.IsDebuggerLoaded ? DebuggerService.CurrentDebugger.IsProcessRunning : false;
			
			bool isdebuggingPassed = (isdebugging == String.Empty) ||
				(debuggerIsDebugging == Boolean.Parse(isdebugging));

			bool isprocessrunningPassed = (isprocessrunning == String.Empty) ||
				(debuggerIsProcessRunning == Boolean.Parse(isprocessrunning));
			
			return isdebuggingPassed && isprocessrunningPassed;
		}
	}
}
