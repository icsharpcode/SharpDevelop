// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Implements the IPadDescriptor interface and
	/// wraps access to the actual PadDescriptor for the
	/// Output Window pad.
	/// </summary>
	public class PythonOutputWindowPadDescriptor : IPadDescriptor
	{
		PadDescriptor pad;
		
		public PythonOutputWindowPadDescriptor()
		{
			pad = WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
		}
		
		public void BringPadToFront()
		{
			pad.BringPadToFront();
		}
	}
}
