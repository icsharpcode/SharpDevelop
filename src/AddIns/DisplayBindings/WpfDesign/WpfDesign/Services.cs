// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3246 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	#region ISelectionService
	/// <summary>
	/// Defines the type how a selection can be changed.
	/// </summary>
	[Flags]
	public enum SelectionTypes
	{
		/// <summary>
		/// No selection type specified.
		/// </summary>
		None = 0,
		/// <summary>
		/// Automatically determine the selection type using the currently pressed
		/// modifier keys.
		/// </summary>
		Auto = 1,
		/// <summary>
		/// Change the primary selection only.
		/// </summary>
		Primary = 2,
		/// <summary>
		/// Toggle the selection.
		/// </summary>
		Toggle = 4,
		/// <summary>
		/// Add to the selection.
		/// </summary>
		Add = 8,
		/// <summary>
		/// Remove from the selection.
		/// </summary>
		Remove = 0x10,
		/// <summary>
		/// Replace the selection.
		/// </summary>
		Replace = 0x20
	}

	/// <summary>
	/// Manages selecting components.
	/// </summary>
	public interface ISelectionService
	{
		/// <summary>Occurs when the current selection is about to change.</summary>
		event EventHandler SelectionChanging;

		/// <summary>Occurs after the current selection has changed.</summary>
		event EventHandler<DesignItemCollectionEventArgs> SelectionChanged;

		/// <summary>Occurs when the primary selection is about to change.</summary>
		event EventHandler PrimarySelectionChanging;
		/// <summary>Occurs after the primary selection has changed.</summary>
		event EventHandler PrimarySelectionChanged;

		/// <summary>
		/// Gets if the specified component is selected.
		/// </summary>
		bool IsSelected(DesignItem item);

		/// <summary>
		/// Gets the collection of selected components.
		/// This is a copy of the actual selected components collection, the returned copy
		/// of the collection will not reflect future changes to the selection.
		/// </summary>
		IEnumerable<DesignItem> SelectedItems { get; }

		/// <summary>Gets the count of selected objects.</summary>
		/// <returns>The number of selected objects.</returns>
		int SelectionCount { get; }

		/// <summary>
		/// Replaces the current selection with the specified selection.
		/// </summary>
		void Select(IEnumerable<DesignItem> items);

		/// <summary>
		/// Modifies the current selection using the specified components and selectionType.
		/// </summary>
		void Select(IEnumerable<DesignItem> items, SelectionTypes selectionType);

		/// <summary>Gets the object that is currently the primary selected object.</summary>
		/// <returns>The object that is currently the primary selected object.</returns>
		DesignItem PrimarySelection { get; }
	}
	#endregion

	#region IViewService
	/// <summary>
	/// Service for getting the view for a model or the model for a view.
	/// </summary>
	public interface IViewService
	{
		/// <summary>
		/// Gets the model represented by the specified view element.
		/// </summary>
		DesignItem GetModel(DependencyObject view);

		/// <summary>
		/// Gets the view for the specified model item.
		/// This is equivalent to using <c>model.View</c>.
		/// </summary>
		DependencyObject GetView(DesignItem model);
	}
	#endregion

	#region IPropertyDescriptionService
	/// <summary>
	/// Used to get a description for properties.
	/// </summary>
	public interface IPropertyDescriptionService
	{
		/// <summary>
		/// Gets a WPF object representing a graphical description of the property.
		/// </summary>
		object GetDescription(DesignItemProperty property);
	}
	#endregion

	#region IErrorService
	/// <summary>
	/// Service for showing error UI.
	/// </summary>
	public interface IErrorService
	{
		/// <summary>
		/// Shows an error tool tip.
		/// </summary>
		void ShowErrorTooltip(FrameworkElement attachTo, UIElement errorElement);
	}
	#endregion

	#region IEventHandlerService
	/// <summary>
	/// Service for providing the designer with information about available event handlers.
	/// </summary>
	public interface IEventHandlerService
	{
		/// <summary>
		/// Creates an event handler for the specified event.
		/// </summary>
		void CreateEventHandler(DesignItemProperty eventProperty);

		/// <summary>
		/// Gets the default event of the specified design item.
		/// </summary>
		DesignItemProperty GetDefaultEvent(DesignItem item);
	}
	#endregion

	#region ITopLevelWindowService
	/// <summary>
	/// Represents a top level window.
	/// </summary>
	public interface ITopLevelWindow
	{
		/// <summary>
		/// Sets child.Owner to the top level window.
		/// </summary>
		void SetOwner(Window child);

		/// <summary>
		/// Activates the window.
		/// </summary>
		bool Activate();
	}

	/// <summary>
	/// Provides a method to get the top-level-window of any UIElement.
	/// If the WPF Designer is hosted inside a Windows.Forms application, the hosting environment
	/// should specify a ITopLevelWindowService implementation that works with <b>both</b> WPF and Windows.Forms
	/// top-level-windows.
	/// </summary>
	public interface ITopLevelWindowService
	{
		/// <summary>
		/// Gets the top level window that contains the specified element.
		/// </summary>
		ITopLevelWindow GetTopLevelWindow(UIElement element);
	}
	#endregion

	#region IUndoService

	public interface IUndoAction
	{
		/// <summary>
		/// The list of items affected by the action.
		/// </summary>
		IEnumerable<DesignItem> AffectedItems { get; }

		/// <summary>
		/// The title of the action.
		/// </summary>
		string Title { get; }

		void Do();
		void Undo();
	}

	public interface IChangeGroup : IUndoAction, IDisposable
	{
		void Commit();
		void Abort();
	}

	public interface ITextContainer
	{
		string Text { get; set; }
	}

	public interface ITextAction : IUndoAction
	{
	}

	public interface IUndoService
	{
		bool CanRedo { get; }
		bool CanUndo { get; }
		IEnumerable<IUndoAction> RedoActions { get; }
		IEnumerable<IUndoAction> UndoActions { get; }
		event EventHandler UndoStackChanged;

		void Undo();
		void Redo();
		void Clear();

		void Execute(IUndoAction action);
		void Done(IUndoAction action);

		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// </summary>
		IChangeGroup OpenGroup(string title, IEnumerable<DesignItem> affectedItems);
	}

	#endregion

	#region IModelService

	public interface IModelService
	{
		/// <summary>
		/// Gets the root design item.
		/// </summary>
		DesignItem Root { get; set; }

		event EventHandler RootChanged;

		event ModelChangedEventHandler ModelChanged;

		event EventHandler<DesignItemEventArgs> ItemCreated;

		DesignItem CreateItem(object component);
	}

	public delegate void ModelChangedEventHandler(object sender, ModelChangedEventArgs e);

	public class ModelChangedEventArgs : EventArgs
	{
		public DesignItemProperty Property;
		public DesignItem OldItem;
		public DesignItem NewItem;
	}

	#endregion

	#region ICommandService

	public interface ICommandService
	{
		bool CanUndo();
		bool CanRedo();
		bool CanCopy();
		bool CanPaste();
		bool CanCut();
		bool CanSelectAll();
		bool CanDelete();

		void Undo();
		void Redo();
		void Copy();
		void Paste();
		void Cut();
		void SelectAll();
		void Delete();
	}

	#endregion
}
