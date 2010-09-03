// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.RubyBinding
{
	public class RunDebugRubyCommand : RunRubyCommand
	{
		public RunDebugRubyCommand(IScriptingWorkbench workbench, RubyAddInOptions options, IDebugger debugger) 
			: base(workbench, options, debugger)
		{
			Debug = true;
		}
		
		public RunDebugRubyCommand()
			: this(new RubyWorkbench(), new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
	}
}
