// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
