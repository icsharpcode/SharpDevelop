// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
				if (configureSideBarDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					ToolboxProvider.ReloadSideTabs(true);
				}
			}
		}
	}
}
