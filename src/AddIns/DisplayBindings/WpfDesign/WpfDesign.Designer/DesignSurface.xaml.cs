using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICSharpCode.WpfDesign.Designer
{
	public partial class DesignSurface
	{
		public DesignSurface()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ContextProperty =
			DependencyProperty.Register("Context", typeof(DesignContext), typeof(DesignSurface));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (Context != null && e.OriginalSource == uxZoom.ScrollViewer)
			{
				UnselectAll();
				Context.ToolService.Reset();
			}
		}

		void UnselectAll()
		{
			Context.SelectionService.Select(null);
		}

		#region Commands

		public bool HasContext()
		{
			return Context != null;
		}

		public bool CanUndo()
		{
			return Context != null && Context.UndoService.CanUndo;
		}

		public void Undo()
		{
			IUndoAction action = Context.UndoService.UndoActions.First();
			Context.UndoService.Undo();
			Context.SelectionService.Select(ModelTools.GetLiveElements(action.AffectedItems));
		}

		public bool CanRedo()
		{
			return Context != null && Context.UndoService.CanRedo;
		}

		public void Redo()
		{
			IUndoAction action = Context.UndoService.RedoActions.First();
			Context.UndoService.Redo();
			Context.SelectionService.Select(ModelTools.GetLiveElements(action.AffectedItems));
		}

		public bool CanCopy()
		{
			return false;
		}

		public void Copy()
		{
		}

		public bool CanCut()
		{
			return false;
		}

		public void Cut()
		{
		}

		public bool CanDelete()
		{
			return Context != null && ModelTools.CanDeleteComponents(Context.SelectionService.SelectedItems);
		}

		public void Delete()
		{
			ModelTools.DeleteComponents(Context.SelectionService.SelectedItems);
		}

		public bool CanPaste()
		{
			return false;
		}

		public void Paste()
		{
		}

		//TODO: Do not select layout root
		public void SelectAll()
		{
			var items = ModelTools.Descendants(Context.ModelService.Root)
				.Where(item => ModelTools.CanSelectComponent(item)).ToArray();
			Context.SelectionService.Select(items);
		}

		#endregion
	}
}
