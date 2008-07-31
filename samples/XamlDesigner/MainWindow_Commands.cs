using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ICSharpCode.XamlDesigner
{
	public partial class MainWindow
	{
		public static SimpleCommand CloseAllCommand = new SimpleCommand("Close All");
		public static SimpleCommand SaveAllCommand = new SimpleCommand("Save All", ModifierKeys.Control | ModifierKeys.Shift, Key.S);
		public static SimpleCommand ExitCommand = new SimpleCommand("Exit");

		static void RenameCommands()
		{
			ApplicationCommands.Open.Text = "Open...";
			ApplicationCommands.SaveAs.Text = "Save As...";
		}

		void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.New();
		}

		void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Open();
		}

		void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseCommand_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseAll();
		}

		void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocument();
		}

		void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocumentAs();
		}

		void SaveAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveAll();
		}

		void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Exit();
		}

		void CurrentDocument_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Shell.Instance.CurrentDocument != null;
		}
	}
}
