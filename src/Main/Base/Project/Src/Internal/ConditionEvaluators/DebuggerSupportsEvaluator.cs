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
			var debugger = SD.Debugger;
			switch (condition.Properties["debuggersupports"]) {
				case "Start":
					return debugger.Supports(DebuggerFeatures.Start);
				case "StartWithoutDebugging":
					return debugger.Supports(DebuggerFeatures.StartWithoutDebugging);
				case "Stop":
					return debugger.Supports(DebuggerFeatures.Stop);
				case "ExecutionControl":
					return debugger.Supports(DebuggerFeatures.ExecutionControl);
				case "Stepping":
					return debugger.Supports(DebuggerFeatures.Stepping);
				case "Attaching":
					return debugger.Supports(DebuggerFeatures.Attaching);
				case "Detaching":
					return debugger.Supports(DebuggerFeatures.Detaching);
				default:
					throw new ArgumentException("Unknown debugger support for : >" + condition.Properties["debuggersupports"] + "< please fix addin file.", "debuggersupports");
			}
		}
	}
}
