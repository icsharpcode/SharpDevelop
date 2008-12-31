using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Commanding;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignCommands : ViewModel
	{
		internal DesignCommands(DesignContext context)
		{
			this.context = context;

			AddCommand("Undo", Undo, CanUndo);
			AddCommand("Redo", Redo, CanRedo);
			AddCommand("Copy", Copy, CanCopy);
			AddCommand("Cut", Cut, CanCut);
			AddCommand("Paste", Paste, CanPaste);
			AddCommand("Delete", Delete, CanDelete);
			AddCommand("SelectAll", SelectAll, CanSelectAll);
			AddCommand("BringToFront", BringToFront, CanBringToFront);
			AddCommand("SendToBack", SendToBack, CanSendToBack);
		}

		DesignContext context;		

		public bool CanUndo()
		{
			return context.UndoManager.CanUndo;
		}

		public void Undo()
		{
			context.UndoManager.Undo();
		}

		public bool CanRedo()
		{
			return context.UndoManager.CanRedo;
		}

		public void Redo()
		{
			context.UndoManager.Redo();
		}

		public bool CanCopy()
		{
			return false;
		}

		public void Copy()
		{
			throw new NotImplementedException();
		}

		public bool CanPaste()
		{
			return false;
		}

		public void Paste()
		{
			throw new NotImplementedException();
		}

		public bool CanCut()
		{
			return false;
		}

		public void Cut()
		{
			throw new NotImplementedException();
		}

		public bool CanSelectAll()
		{
			return true;
		}

		public void SelectAll()
		{
			context.Selection.Set(context.Root.Descendants().Skip(1));
		}

		public bool CanDelete()
		{
			return context.Selection.Count > 0 && !context.Selection.Contains(context.Root);
		}

		public void Delete()
		{
			context.Selection.Delete();
		}

		public bool CanBringToFront()
		{
			return true;
		}

		public void BringToFront()
		{
		}

		public bool CanSendToBack()
		{
			return true;
		}

		public void SendToBack()
		{
		}
	}
}
