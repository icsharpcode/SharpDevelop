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
			bool isprocessrunning = Boolean.Parse(condition.Properties["isprocessrunning"]);
			string isDebugging = condition.Properties.Get("isdebugging", String.Empty);
			IDebugger debugger = DebuggerService.CurrentDebugger;
			return isprocessrunning == DebuggerService.IsProcessRuning && 
				   (isDebugging == String.Empty || 
				    DebuggerService.IsDebugging == Boolean.Parse(isDebugging)
				   );
		}
	}
}
