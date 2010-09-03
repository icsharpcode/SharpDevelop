// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PythonBinding
{
	public class RunDebugPythonCommand : RunPythonCommand
	{
		public RunDebugPythonCommand(IPythonWorkbench workbench, PythonAddInOptions options, IDebugger debugger) 
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
