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
			bool debuggerIsDebugging = SD.Debugger.IsDebuggerLoaded ? SD.Debugger.IsDebugging : false;
			bool debuggerIsProcessRunning = SD.Debugger.IsDebuggerLoaded ? SD.Debugger.IsProcessRunning : false;
			
			bool isdebuggingPassed = (isdebugging == String.Empty) ||
				(debuggerIsDebugging == Boolean.Parse(isdebugging));

			bool isprocessrunningPassed = (isprocessrunning == String.Empty) ||
				(debuggerIsProcessRunning == Boolean.Parse(isprocessrunning));
			
			return isdebuggingPassed && isprocessrunningPassed;
		}
	}
}
