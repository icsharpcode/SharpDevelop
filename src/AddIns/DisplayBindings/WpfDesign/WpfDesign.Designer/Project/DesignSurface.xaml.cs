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
using System.Xml;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Surface hosting the WPF designer.
	/// </summary>
	public partial class DesignSurface
	{
		public DesignSurface()
		{
			InitializeComponent();

			this.AddCommandHandler(ApplicationCommands.Undo, Undo, CanUndo);
			this.AddCommandHandler(ApplicationCommands.Redo, Redo, CanRedo);
			this.AddCommandHandler(ApplicationCommands.Copy, Copy, HasSelection);
			this.AddCommandHandler(ApplicationCommands.Cut, Cut, HasSelection);
			this.AddCommandHandler(ApplicationCommands.Delete, Delete, CanDelete);
			this.AddCommandHandler(ApplicationCommands.Paste, Paste, CanPaste);
			this.AddCommandHandler(ApplicationCommands.SelectAll, SelectAll, CanSelectAll);
		}

		DesignContext _designContext;

		/// <summary>
		/// Gets the active design context.
		/// </summary>
		public DesignContext DesignContext {
			get { return _designContext; }
		}
		
		/// <summary>
		/// Initializes the designer content from the specified XmlReader.
		/// </summary>
		public void LoadDesigner(XmlReader xamlReader, XamlLoadSettings loadSettings)
		{
			UnloadDesigner();
			InitializeDesigner(new XamlDesignContext(xamlReader, loadSettings ?? new XamlLoadSettings()));
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
			_sceneContainer.Child = context.RootItem.View;
			
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
			_sceneContainer.Child = null;
			_designPanel.Adorners.Clear();
		}

		#region Commands

		public bool CanUndo()
		{
			UndoService undoService = GetService<UndoService>();
			return undoService != null && undoService.CanUndo;
		}

		public void Undo()
		{
			UndoService undoService = GetService<UndoService>();
			IUndoAction action = undoService.UndoActions.First();
			Debug.WriteLine("Undo " + action.Title);
			undoService.Undo();
			_designContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
		}

		public bool CanRedo()
		{
			UndoService undoService = GetService<UndoService>();
			return undoService != null && undoService.CanRedo;
		}

		public void Redo()
		{
			UndoService undoService = GetService<UndoService>();
			IUndoAction action = undoService.RedoActions.First();
			Debug.WriteLine("Redo " + action.Title);
			undoService.Redo();
			_designContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
		}

		public bool HasSelection()
		{
			return false;
		}

		public void Copy()
		{
		}

		public void Cut()
		{
		}

		public bool CanDelete()
		{
			if (_designContext != null) {
				return ModelTools.CanDeleteComponents(_designContext.Services.Selection.SelectedItems);
			}
			return false;
		}

		public void Delete()
		{
			if (_designContext != null) {
				ModelTools.DeleteComponents(_designContext.Services.Selection.SelectedItems);
			}
		}

		public bool CanPaste()
		{
			return false;
		}

		public void Paste()
		{
		}

		public bool CanSelectAll()
		{
			return DesignContext != null;
		}

		//TODO: Do not select layout root
		public void SelectAll()
		{
			var items = Descendants(DesignContext.RootItem).Where(item => ModelTools.CanSelectComponent(item)).ToArray();
			DesignContext.Services.Selection.SetSelectedComponents(items);
		}

		//TODO: Share with Outline / PlacementBehavior
		public static IEnumerable<DesignItem> DescendantsAndSelf(DesignItem item)
		{
			yield return item;
			foreach (var child in Descendants(item)) {
				yield return child;
			}			
		}

		public static IEnumerable<DesignItem> Descendants(DesignItem item)
		{
			if (item.ContentPropertyName != null) {
				var content = item.ContentProperty;
				if (content.IsCollection) {
					foreach (var child in content.CollectionElements) {
						foreach (var child2 in DescendantsAndSelf(child)) {
							yield return child2;
						}
					}
				} else {
					if (content.Value != null) {
						foreach (var child2 in DescendantsAndSelf(content.Value)) {
							yield return child2;
						}
					}
				}
			}
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

		T GetService<T>() where T : class
		{
			if (_designContext != null)
				return _designContext.Services.GetService<T>();
			else
				return null;
		}

		#endregion
	}
}
