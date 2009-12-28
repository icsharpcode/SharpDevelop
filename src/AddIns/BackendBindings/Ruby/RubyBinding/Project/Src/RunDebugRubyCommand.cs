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

namespace ICSharpCode.RubyBinding
{
	public class RunDebugRubyCommand : RunRubyCommand
	{
		public RunDebugRubyCommand(IWorkbench workbench, RubyAddInOptions options, IDebugger debugger) 
			: base(workbench, options, debugger)
		{
			Debug = true;
		}
		
		public RunDebugRubyCommand()
			: this(WorkbenchSingleton.Workbench, new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
	}
}
