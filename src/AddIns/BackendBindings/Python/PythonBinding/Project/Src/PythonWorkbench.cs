// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		public IPythonConsolePad GetPythonConsolePad()
		{
			PadDescriptor padDescriptor = workbench.GetPad(typeof(PythonConsolePad));
			return padDescriptor.PadContent as IPythonConsolePad;
		}
	}
}
