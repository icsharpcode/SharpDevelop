// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	public class RubyWorkbench : IRubyWorkbench
	{
		IWorkbench workbench;
		
		public RubyWorkbench()
		{
			workbench = WorkbenchSingleton.Workbench;
		}		
		
		public IViewContent ActiveViewContent {
			get { return workbench.ActiveViewContent; }
		}
		
		public IRubyConsolePad GetRubyConsolePad()
		{
			PadDescriptor padDescriptor = workbench.GetPad(typeof(RubyConsolePad));
			return padDescriptor.PadContent as IRubyConsolePad;
		}
	}
}
