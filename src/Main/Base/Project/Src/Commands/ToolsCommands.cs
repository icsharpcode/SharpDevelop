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
		public static bool? ShowTabbedOptions(string dialogTitle, AddInTreeNode node)
		{
			TabbedOptionsDialog o = new TabbedOptionsDialog(node.BuildChildItems<IOptionPanelDescriptor>(null));
			o.Title = dialogTitle;
			o.Owner = WorkbenchSingleton.MainWindow;
			return o.ShowDialog();
		}
		
		public static bool? ShowTreeOptions(string dialogTitle, AddInTreeNode node)
		{
			TreeViewOptionsDialog o = new TreeViewOptionsDialog(node.BuildChildItems<IOptionPanelDescriptor>(null));
			o.Title = dialogTitle;
			o.Owner = WorkbenchSingleton.MainWindow;
			return o.ShowDialog();
		}
		
		public override void Run()
		{
			bool? result = ShowTreeOptions(
				ResourceService.GetString("Dialog.Options.TreeViewOptions.DialogName"),
				AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"));
			if (result == true) {
				// save properties after changing options
				PropertyService.Save();
			}
		}
	}
	
	public class ToggleFullscreenCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.FullScreen = !WorkbenchSingleton.Workbench.FullScreen;
		}
	}
}
