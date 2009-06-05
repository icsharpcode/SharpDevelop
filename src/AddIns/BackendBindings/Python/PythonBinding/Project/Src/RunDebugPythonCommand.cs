// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PythonBinding
{
	public class RunDebugPythonCommand : RunPythonCommand
	{
		public RunDebugPythonCommand(IWorkbench workbench, AddInOptions options, IDebugger debugger) 
			: base(workbench, options, debugger)
		{
			Debug = true;
		}
		
		public RunDebugPythonCommand()
			: this(WorkbenchSingleton.Workbench, new AddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
	}
}
