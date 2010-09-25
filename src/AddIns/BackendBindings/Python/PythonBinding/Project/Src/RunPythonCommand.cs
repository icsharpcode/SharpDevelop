// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Runs the Python console passing the filename of the 
	/// active python script open in SharpDevelop.
	/// </summary>
	public class RunPythonCommand : RunScriptingConsoleApplicationCommand
	{
		public RunPythonCommand()
			: this(new PythonWorkbench(), new PythonAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunPythonCommand(IScriptingWorkbench workbench, PythonAddInOptions options, IDebugger debugger)
			: base(workbench, debugger, new PythonConsoleApplication(options))
		{
		}
	}
}
