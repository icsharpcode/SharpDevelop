// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.PythonBinding
{
	public class RunDebugPythonCommand : RunPythonCommand
	{
		public RunDebugPythonCommand(IScriptingWorkbench workbench, PythonAddInOptions options, IDebugger debugger) 
			: base(workbench, options, debugger)
		{
			Debug = true;
		}
		
		public RunDebugPythonCommand()
			: this(new PythonWorkbench(), new PythonAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
	}
}
