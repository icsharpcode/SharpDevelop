// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Switches to the XAML source code tab.
	/// </summary>
	public class ViewXaml : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SwitchView(0);
		}
	}
}
