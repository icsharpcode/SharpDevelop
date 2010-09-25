// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Runs the Ruby console passing the filename of the 
	/// active Ruby script open in SharpDevelop.
	/// </summary>
	public class RunRubyCommand : RunScriptingConsoleApplicationCommand
	{		
		public RunRubyCommand()
			: this(new RubyWorkbench(), new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunRubyCommand(IScriptingWorkbench workbench, RubyAddInOptions options, IDebugger debugger)
			: base(workbench, debugger, new RubyConsoleApplication(options))
		{
		}
	}
}
