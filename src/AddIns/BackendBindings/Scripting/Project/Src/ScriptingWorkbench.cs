// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public class ScriptingWorkbench : IScriptingWorkbench
	{
		Type consolePadType;
		IWorkbench workbench;
		
		public ScriptingWorkbench(Type consolePadType)
		{
			this.consolePadType = consolePadType;
			workbench = WorkbenchSingleton.Workbench;
		}		
		
		public IViewContent ActiveViewContent {
			get { return workbench.ActiveViewContent; }
		}
		
		public IScriptingConsolePad GetScriptingConsolePad()
		{
			PadDescriptor padDescriptor = workbench.GetPad(consolePadType);
			return padDescriptor.PadContent as IScriptingConsolePad;
		}
	}
}
