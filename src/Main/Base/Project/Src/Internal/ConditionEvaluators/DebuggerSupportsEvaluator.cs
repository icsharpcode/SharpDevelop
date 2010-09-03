// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Tests if the debugger supports a certain feature.
	/// </summary>
	/// <attribute name="debuggersupports">
	/// The name of the feature the debugger must support.
	/// Possible feature names: "Start", "StartWithoutDebugging", "Stop",
	/// "ExecutionControl", "Stepping".
	/// </attribute>
	/// <example title="Test if the debugger supports stepping">
	/// &lt;Condition name = "DebuggerSupports" debuggersupports="Stepping"&gt;
	/// </example>
	/// <example title="Test if the debugger supports killing the running application">
	/// &lt;Condition name = "DebuggerSupports" debuggersupports="Stop"&gt;
	/// </example>
	public class DebuggerSupportsConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			DebuggerDescriptor debugger = DebuggerService.Descriptor;
			switch (condition.Properties["debuggersupports"]) {
				case "Start":
					return (debugger != null) ? debugger.SupportsStart : true;
				case "StartWithoutDebugging":
					return (debugger != null) ? debugger.SupportsStartWithoutDebugging : true;
				case "Stop":
					return (debugger != null) ? debugger.SupportsStop : true;
				case "ExecutionControl":
					return (debugger != null) ? debugger.SupportsExecutionControl : false;
				case "Stepping":
					return (debugger != null) ? debugger.SupportsStepping : false;
				case "Attaching":
					return (debugger != null) ? debugger.SupportsAttaching : false;
				case "Detaching":
					return (debugger != null) ? debugger.SupportsDetaching : false;					
				default:
					throw new ArgumentException("Unknown debugger support for : >" + condition.Properties["debuggersupports"] + "< please fix addin file.", "debuggersupports");
			}
		}
	}
}
