// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	public class PythonWorkbench : IPythonWorkbench
	{
		IWorkbench workbench;
		
		public PythonWorkbench()
		{
			workbench = WorkbenchSingleton.Workbench;
		}		
		
		public IViewContent ActiveViewContent {
			get { return workbench.ActiveViewContent; }
		}
		
		public IScriptingConsolePad GetScriptingConsolePad()
		{
			PadDescriptor padDescriptor = workbench.GetPad(typeof(PythonConsolePad));
			return padDescriptor.PadContent as IScriptingConsolePad;
		}
	}
}
