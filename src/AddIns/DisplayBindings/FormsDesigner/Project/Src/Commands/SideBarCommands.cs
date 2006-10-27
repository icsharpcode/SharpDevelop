// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;

namespace ICSharpCode.FormsDesigner
{
	public class CustomizeSideBar : AbstractMenuCommand
	{
		public override void Run()		
		{
			ConfigureSideBarDialog configureSideBarDialog = new ConfigureSideBarDialog();
			configureSideBarDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			ToolboxProvider.ReloadSideTabs(true);
		}
	}
}
