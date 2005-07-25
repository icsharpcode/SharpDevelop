// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
