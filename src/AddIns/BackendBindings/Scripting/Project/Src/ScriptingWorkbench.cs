// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
