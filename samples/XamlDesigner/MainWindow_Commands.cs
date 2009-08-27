using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace ICSharpCode.XamlDesigner
{
	public partial class MainWindow
	{
		public static SimpleCommand CloseAllCommand = new SimpleCommand("Close All");
		public static SimpleCommand SaveAllCommand = new SimpleCommand("Save All", ModifierKeys.Control | ModifierKeys.Shift, Key.S);
		public static SimpleCommand ExitCommand = new SimpleCommand("Exit");
		public static SimpleCommand RefreshCommand = new SimpleCommand("Refresh", Key.F5);

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

		void RouteDesignSurfaceCommands()
		{
			RouteDesignSurfaceCommand(ApplicationCommands.Undo);
			RouteDesignSurfaceCommand(ApplicationCommands.Redo);
			RouteDesignSurfaceCommand(ApplicationCommands.Copy);
			RouteDesignSurfaceCommand(ApplicationCommands.Cut);
			RouteDesignSurfaceCommand(ApplicationCommands.Paste);
			RouteDesignSurfaceCommand(ApplicationCommands.SelectAll);
			RouteDesignSurfaceCommand(ApplicationCommands.Delete);
		}

		void RouteDesignSurfaceCommand(RoutedCommand command)
		{
			var cb = new CommandBinding(command);
			cb.CanExecute += delegate(object sender, CanExecuteRoutedEventArgs e) {
				if (Shell.Instance.CurrentDocument != null) {
					Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
				}else {
					e.CanExecute = false;
				}
			};
			cb.Executed += delegate(object sender, ExecutedRoutedEventArgs e) {
				Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
			};
			CommandBindings.Add(cb);
		}
	}
}
