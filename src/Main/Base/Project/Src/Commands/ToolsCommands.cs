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
			TabbedOptionsDialog o = new TabbedOptionsDialog(node.BuildChildItems<IOptionPanelDescriptor>(null));
			o.Title = dialogTitle;
			o.Owner = WorkbenchSingleton.MainWindow;
			o.ShowDialog();
		}
		
		public static void ShowTreeOptions(string dialogTitle, AddInTreeNode node)
		{
			TreeViewOptionsDialog o = new TreeViewOptionsDialog(node.BuildChildItems<IOptionPanelDescriptor>(null));
			o.Title = dialogTitle;
			o.Owner = WorkbenchSingleton.MainWindow;
			o.ShowDialog();
		}
		
		public override void Run()
		{
			ShowTreeOptions(
				ResourceService.GetString("Dialog.Options.TreeViewOptions.DialogName"),
				AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"));
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
