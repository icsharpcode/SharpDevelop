// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
		public static Type[] SupportedToolboxControls = {
			typeof(Button),
			typeof(CheckBox),
			typeof(ComboBox),
			typeof(Label),
			typeof(TextBox),
			typeof(RadioButton),
			typeof(Canvas),
			typeof(Grid),
			typeof(Border),
			typeof(DockPanel),
			typeof(Expander),
			typeof(GroupBox),
			typeof(Image),
			typeof(InkCanvas),
			typeof(ListBox),
			typeof(ListView),
			typeof(Menu),
			typeof(PasswordBox),
			typeof(ProgressBar),
			typeof(RichTextBox),
			typeof(ScrollViewer),
			typeof(Slider),
			typeof(StackPanel),
			typeof(TabControl),
			typeof(ToolBar),
			typeof(TreeView),
			typeof(Viewbox),
			typeof(Viewport3D),
			typeof(WrapPanel)
		};
		
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
			this.DataContext = null;
			
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
			IUndoAction action = undoService.UndoActions.First();
			Debug.WriteLine("Undo " + action.Title);
			undoService.Undo();
			_designContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
		}
		
		void OnUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			e.CanExecute = undoService != null && undoService.CanUndo;
		}
		
		void OnRedoExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			IUndoAction action = undoService.RedoActions.First();
			Debug.WriteLine("Redo " + action.Title);
			undoService.Redo();
			_designContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
		}
		
		void OnRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			UndoService undoService = GetService<UndoService>();
			e.CanExecute = undoService != null && undoService.CanRedo;
		}
		
		// Filters an element list, dropping all elements that are not part of the xaml document
		// (e.g. because they were deleted).
		static List<DesignItem> GetLiveElements(ICollection<DesignItem> items)
		{
			List<DesignItem> result = new List<DesignItem>(items.Count);
			foreach (DesignItem item in items) {
				if (ModelTools.IsInDocument(item) && ModelTools.CanSelectComponent(item)) {
					result.Add(item);
				}
			}
			return result;
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
		public void LoadDesigner(XmlReader xamlReader, Xaml.XamlLoadSettings loadSettings)
		{
			UnloadDesigner();
			InitializeDesigner(new Xaml.XamlDesignContext(xamlReader, loadSettings ?? new Xaml.XamlLoadSettings()));
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
				undoService => undoService.UndoStackChanged += delegate {
					CommandManager.InvalidateRequerySuggested();
				}
			);
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
