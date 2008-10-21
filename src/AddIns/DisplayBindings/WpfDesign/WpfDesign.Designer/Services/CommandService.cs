using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	class CommandService : ICommandService
	{
		public CommandService(DesignContext context)
		{
			this.Context = context;
		}

		DesignContext Context;

		public bool CanUndo()
		{
			return Context.UndoService.CanUndo;
		}

		public bool CanRedo()
		{
			return Context.UndoService.CanRedo;
		}

		public bool CanCopy()
		{
			return false;
		}

		public bool CanPaste()
		{
			return false;
		}

		public bool CanCut()
		{
			return false;
		}

		public bool CanSelectAll()
		{
			return true;
		}

		public bool CanDelete()
		{
			return ModelTools.CanDeleteComponents(Context.SelectionService.SelectedItems);
		}

		public void Undo()
		{
			IUndoAction action = Context.UndoService.UndoActions.First();
			Context.UndoService.Undo();
			Context.SelectionService.Select(ModelTools.GetLiveElements(action.AffectedItems));
		}

		public void Redo()
		{
			IUndoAction action = Context.UndoService.RedoActions.First();
			Context.UndoService.Redo();
			Context.SelectionService.Select(ModelTools.GetLiveElements(action.AffectedItems));
		}

		public void Copy()
		{
			throw new NotImplementedException();
		}

		public void Paste()
		{
			throw new NotImplementedException();
		}

		public void Cut()
		{
			throw new NotImplementedException();
		}

		//TODO: Do not select layout root
		public void SelectAll()
		{
			var items = ModelTools.Descendants(Context.ModelService.Root)
				.Where(item => ModelTools.CanSelectComponent(item)).ToArray();
			Context.SelectionService.Select(items);
		}

		public void Delete()
		{
			ModelTools.DeleteComponents(Context.SelectionService.SelectedItems);
		}
	}
}
