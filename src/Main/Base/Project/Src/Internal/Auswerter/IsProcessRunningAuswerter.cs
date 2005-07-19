// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class IsProcessRunningAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			string isdebugging = condition.Properties.Get("isdebugging", String.Empty);
			string isprocessrunning = condition.Properties.Get("isprocessrunning", String.Empty);

			bool isdebuggingPassed = (isdebugging == String.Empty) || 
				                     (DebuggerService.CurrentDebugger.IsDebugging == Boolean.Parse(isdebugging));

			bool isprocessrunningPassed = (isprocessrunning == String.Empty) ||
			                              (DebuggerService.CurrentDebugger.IsProcessRunning == Boolean.Parse(isprocessrunning));
			
			return isdebuggingPassed && isprocessrunningPassed;
		}
	}
}
