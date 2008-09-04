// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class OptionsCommand : AbstractMenuCommand
	{
		public static void ShowTabbedOptions(string dialogTitle, AddInTreeNode node)
		{
			using (TabbedOptions o = new TabbedOptions(dialogTitle, node)) {
				o.Width  = 450;
				o.Height = 425;
				o.FormBorderStyle = FormBorderStyle.FixedDialog;
				o.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
		
		public override void Run()
		{
			using (TreeViewOptions optionsDialog = new TreeViewOptions(AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"))) {
				optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
				
				optionsDialog.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
	}
	
	public class ToggleFullscreenCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			throw new NotImplementedException();
//			((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen = !((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen;
		}
	}
}
