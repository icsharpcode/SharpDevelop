// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Input;
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
		bool IsComponentSelected(DesignItem component);
		
		/// <summary>
		/// Gets the collection of selected components.
		/// This is a copy of the actual selected components collection, the returned copy
		/// of the collection will not reflect future changes to the selection.
		/// </summary>
		ICollection<DesignItem> SelectedItems { get; }
		
		/// <summary>
		/// Replaces the current selection with the specified selection.
		/// </summary>
		void SetSelectedComponents(ICollection<DesignItem> components);
		
		/// <summary>
		/// Modifies the current selection using the specified components and selectionType.
		/// </summary>
		void SetSelectedComponents(ICollection<DesignItem> components, SelectionTypes selectionType);
		
		/// <summary>Gets the object that is currently the primary selected object.</summary>
		/// <returns>The object that is currently the primary selected object.</returns>
		DesignItem PrimarySelection { get; }
		
		/// <summary>Gets the count of selected objects.</summary>
		/// <returns>The number of selected objects.</returns>
		int SelectionCount { get; }
	}
	#endregion
	
	#region IComponentService
	/// <summary>Supports adding and removing components</summary>
	public interface IComponentService
	{
		/// <summary>
		/// Gets the site of an existing, registered component.
		/// </summary>
		/// <returns>
		/// The site of the component, or null if the component is not registered.
		/// </returns>
		DesignItem GetDesignItem(object component);
		
		/// <summary>Registers a component for usage in the designer.</summary>
		DesignItem RegisterComponentForDesigner(object component);
		
		/// <summary>Event raised whenever a component is registered</summary>
		event EventHandler<DesignItemEventArgs> ComponentRegistered;
		
		/// <summary>Property Changed</summary>
		event EventHandler<DesignItemPropertyChangedEventArgs> PropertyChanged;

		/// <summary> Set's default Property Values as defined in Metadata </summary>
		void SetDefaultPropertyValues(DesignItem designItem);
	}
	#endregion
	
	#region IViewService
	/// <summary>
	/// Service for getting the view for a model or the model for a view.
	/// </summary>
	public abstract class ViewService
	{
		/// <summary>
		/// Gets the model represented by the specified view element.
		/// </summary>
		public abstract DesignItem GetModel(DependencyObject view);
		
		/// <summary>
		/// Gets the view for the specified model item.
		/// This is equivalent to using <c>model.View</c>.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public DependencyObject GetView(DesignItem model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			return model.View;
		}
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
		object GetDescription(DesignItemProperty designProperty);
	}
	#endregion
	
	#region IOutlineNodeNameService
	/// <summary>
	/// Used to get a description for the Outline Node.
	/// </summary>
	public interface IOutlineNodeNameService
	{
		/// <summary>
		/// Gets a the Name for display in the Ouline Node.
		/// </summary>
		string GetOutlineNodeName(DesignItem designItem);
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
	
	#region IKeyBindingService
	
	/// <summary>
	/// Service that handles all the key bindings in the designer.
	/// </summary>
	public interface IKeyBindingService
	{
		/// <summary>
		/// Gets the object to which the bindings are being applied
		/// </summary>
		object Owner { get; }

		/// <summary>
		/// Register <paramref name="binding"/> with <see cref="Owner"/>.
		/// </summary>
		/// <param name="binding">The binding to be applied.</param>
		void RegisterBinding(KeyBinding binding);

		/// <summary>
		/// De-register <paramref name="binding"/> with <see cref="Owner"/>.
		/// </summary>
		/// <param name="binding">The binding to be applied.</param>
		void DeregisterBinding(KeyBinding binding);

		/// <summary>
		/// Gets binding for the corresponding gesture otherwise returns null.
		/// </summary>
		/// <param name="gesture">The keyboard gesture requested.</param>
		KeyBinding GetBinding(KeyGesture gesture);
	}
	
	#endregion
}
