using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign;

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

		void RegisterCommandHandlers()
		{
			this.AddCommandHandler(ApplicationCommands.New, Shell.Instance.New);
			this.AddCommandHandler(ApplicationCommands.Open, Shell.Instance.Open);
			this.AddCommandHandler(ApplicationCommands.Close, Shell.Instance.CloseCurrentDocument, HasCurrentDocument);
			this.AddCommandHandler(ApplicationCommands.Save, Shell.Instance.SaveCurrentDocument, HasCurrentDocument);
			this.AddCommandHandler(ApplicationCommands.SaveAs, Shell.Instance.SaveCurrentDocumentAs, HasCurrentDocument);
			
			this.AddCommandHandler(SaveAllCommand, SaveAll, HasCurrentDocument);
			this.AddCommandHandler(CloseAllCommand, CloseAll, HasCurrentDocument);
			this.AddCommandHandler(ExitCommand, Shell.Instance.Exit, HasCurrentDocument);
			this.AddCommandHandler(RefreshCommand, Shell.Instance.Refresh, Shell.Instance.CanRefresh);

			this.AddCommandHandler(ApplicationCommands.Undo, Undo, CanUndo);
			this.AddCommandHandler(ApplicationCommands.Redo, Redo, CanRedo);
			this.AddCommandHandler(ApplicationCommands.Copy, Copy, CanCopy);
			this.AddCommandHandler(ApplicationCommands.Cut, Cut, CanCut);
			this.AddCommandHandler(ApplicationCommands.Delete, Delete, CanDelete);
			this.AddCommandHandler(ApplicationCommands.Paste, Paste, CanPaste);
			this.AddCommandHandler(ApplicationCommands.SelectAll, SelectAll, CanSelectAll);
		}

		bool HasCurrentDocument()
		{
			return Shell.Instance.CurrentDocument != null;
		}

		void SaveAll()
		{
			Shell.Instance.SaveAll();
		}

		void CloseAll()
		{
			Shell.Instance.CloseAll();
		}

		ICommandService CurrentCommandService
		{
			get 
			{
				if (Shell.Instance.CurrentDocument != null) {
					return Shell.Instance.CurrentDocument.Context.CommandService;
				}
				return null;
			}
		}

		void Undo()
		{
			CurrentCommandService.Undo();
		}

		void Redo()
		{
			CurrentCommandService.Redo();
		}

		void Copy()
		{
			CurrentCommandService.Copy();
		}

		void Paste()
		{
			CurrentCommandService.Paste();
		}

		void Cut()
		{
			CurrentCommandService.Cut();
		}

		void SelectAll()
		{
			CurrentCommandService.SelectAll();
		}

		void Delete()
		{
			CurrentCommandService.Delete();
		}

		bool CanUndo()
		{
			return CurrentCommandService != null && CurrentCommandService.CanUndo();
		}

		bool CanRedo()
		{
			return CurrentCommandService != null && CurrentCommandService.CanRedo();
		}

		bool CanCopy()
		{
			return CurrentCommandService != null && CurrentCommandService.CanCopy();
		}

		bool CanPaste()
		{
			return CurrentCommandService != null && CurrentCommandService.CanPaste();
		}

		bool CanCut()
		{
			return CurrentCommandService != null && CurrentCommandService.CanCut();
		}

		bool CanSelectAll()
		{
			return CurrentCommandService != null && CurrentCommandService.CanSelectAll();
		}

		bool CanDelete()
		{
			return CurrentCommandService != null && CurrentCommandService.CanDelete();
		}
	}
}
