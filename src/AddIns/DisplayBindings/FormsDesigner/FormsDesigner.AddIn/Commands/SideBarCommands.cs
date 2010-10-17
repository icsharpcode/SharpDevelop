// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;

namespace ICSharpCode.FormsDesigner
{
	public class CustomizeSideBar : AbstractMenuCommand
	{
		public override void Run()		
		{
			using (ConfigureSideBarDialog configureSideBarDialog = new ConfigureSideBarDialog()) {
				if (configureSideBarDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					ToolboxProvider.ReloadSideTabs(true);
				}
			}
		}
	}
}
