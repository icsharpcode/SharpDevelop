// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class TestCommand : System.Windows.Input.ICommand
	{
		public event EventHandler CanExecuteChanged
		{ 
			add {} 
			remove {}
		}
		
		public void Execute(object parameter)
		{
			CommandsRegistry.GetRoutedUICommand("SDBuildCommands.BuildSolution").Execute(parameter, WorkbenchSingleton.MainWindow);
			
			System.Windows.MessageBox.Show("test");
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
	
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
			if (result ?? false) {
				// save properties after changing options
				PropertyService.Save();
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
