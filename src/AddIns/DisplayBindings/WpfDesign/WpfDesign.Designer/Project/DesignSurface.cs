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
		}
		
		#region Undo/Redo
		UndoService _undoService;
		
		private UndoService UndoService {
			get { return _undoService; }
			set {
				if (_undoService != null) {
					_undoService.UndoStackChanged -= OnUndoStackChanged;
				}
				_undoService = value;
				if (_undoService != null) {
					_undoService.UndoStackChanged += OnUndoStackChanged;
				}
				CommandManager.InvalidateRequerySuggested();
			}
		}
		
		void OnUndoExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			IUndoAction action = Func.First(_undoService.UndoActions);
			Debug.WriteLine("Undo " + action.Title);
			_undoService.Undo();
			_designContext.Services.Selection.SetSelectedComponents(action.AffectedElements);
		}
		
		void OnUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _undoService != null && _undoService.CanUndo;
		}
		
		void OnRedoExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			IUndoAction action = Func.First(_undoService.RedoActions);
			Debug.WriteLine("Redo " + action.Title);
			_undoService.Redo();
			_designContext.Services.Selection.SetSelectedComponents(action.AffectedElements);
		}
		
		void OnRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _undoService != null && _undoService.CanRedo;
		}
		
		void OnUndoStackChanged(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
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
			UndoService = context.Services.GetService<UndoService>();
		}
		
		/// <summary>
		/// Unloads the designer content.
		/// </summary>
		public void UnloadDesigner()
		{
			_designContext = null;
			_designPanel.Context = null;
			_designPanel.Child = null;
			_designPanel.Adorners.Clear();
			_designPanel.MarkerCanvas.Children.Clear();
			UndoService = null;
		}
	}
}
