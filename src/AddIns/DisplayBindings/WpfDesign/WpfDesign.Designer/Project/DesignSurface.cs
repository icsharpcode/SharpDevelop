// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Xml;

using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Surface hosting the WPF designer.
	/// </summary>
	public sealed class DesignSurface : SingleVisualChildElement
	{
		readonly ScrollViewer _scrollViewer;
		readonly DesignPanel _designPanel;
		DesignContext _designContext;
		
		/// <summary>
		/// Create a new DesignSurface instance.
		/// </summary>
		public DesignSurface()
		{
			_scrollViewer = new ScrollViewer();
			_designPanel = new DesignPanel();
			_scrollViewer.Content = _designPanel;
			_scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			_scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
			this.VisualChild = _scrollViewer;
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndoExecuted, OnUndoCanExecute));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnRedoExecuted, OnRedoCanExecute));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDeleteExecuted, OnDeleteCanExecute));
		}
		
		T GetService<T>() where T : class
		{
			if (_designContext != null)
				return _designContext.Services.GetService<T>();
			else
				return null;
		}
		
		#region Command: Undo/Redo
		void OnUndoExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			IUndoAction action = Func.First(undoService.UndoActions);
			Debug.WriteLine("Undo " + action.Title);
			undoService.Undo();
			_designContext.Services.Selection.SetSelectedComponents(action.AffectedElements);
		}
		
		void OnUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			e.CanExecute = undoService != null && undoService.CanUndo;
		}
		
		void OnRedoExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			IUndoAction action = Func.First(undoService.RedoActions);
			Debug.WriteLine("Redo " + action.Title);
			undoService.Redo();
			_designContext.Services.Selection.SetSelectedComponents(action.AffectedElements);
		}
		
		void OnRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			e.CanExecute = undoService != null && undoService.CanRedo;
		}
		#endregion
		
		#region Command: Delete
		void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (_designContext != null) {
				ModelTools.DeleteComponents(_designContext.Services.Selection.SelectedItems);
			}
		}
		
		void OnDeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (_designContext != null) {
				e.CanExecute = ModelTools.CanDeleteComponents(_designContext.Services.Selection.SelectedItems);
			} else {
				e.CanExecute = false;
			}
		}
		#endregion
		
		/// <summary>
		/// Gets the active design context.
		/// </summary>
		public DesignContext DesignContext {
			get { return _designContext; }
		}
		
		/// <summary>
		/// Initializes the designer content from the specified XmlReader.
		/// </summary>
		public void LoadDesigner(XmlReader xamlReader)
		{
			UnloadDesigner();
			InitializeDesigner(new Xaml.XamlDesignContext(xamlReader));
		}
		
		/// <summary>
		/// Saves the designer content into the specified XmlWriter.
		/// </summary>
		public void SaveDesigner(XmlWriter writer)
		{
			_designContext.Save(writer);
		}
		
		void InitializeDesigner(DesignContext context)
		{
			context.Services.AddService(typeof(IDesignPanel), _designPanel);
			
			_designContext = context;
			_designPanel.Context = context;
			Border designPanelBorder = new Border();
			designPanelBorder.Padding = new Thickness(10);
			_designPanel.Child = designPanelBorder;
			designPanelBorder.Child = context.RootItem.View;
			context.Services.RunWhenAvailable<UndoService>(
				delegate (UndoService undoService) {
					CommandManager.InvalidateRequerySuggested();
				});
			context.Services.Selection.SelectionChanged += delegate {
				CommandManager.InvalidateRequerySuggested();
			};
		}
		
		/// <summary>
		/// Unloads the designer content.
		/// </summary>
		public void UnloadDesigner()
		{
			if (_designContext != null) {
				foreach (object o in _designContext.Services.AllServices) {
					IDisposable d = o as IDisposable;
					if (d != null) d.Dispose();
				}
			}
			_designContext = null;
			_designPanel.Context = null;
			_designPanel.Child = null;
			_designPanel.Adorners.Clear();
		}
	}
}
