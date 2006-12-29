// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	#region IServiceContainer
	/// <summary>
	/// The <see cref="IServiceContainer"/> is a built-in service that manages the list of services.
	/// You can only add services to it, removing or replacing services is not supported because
	/// many designers depend on keeping their services available.
	/// </summary>
	public interface IServiceContainer : IServiceProvider
	{
		/// <summary>
		/// Adds a new service to the container.
		/// </summary>
		/// <param name="serviceInterface">
		/// The type of the service interface to use as a key for the service.
		/// </param>
		/// <param name="serviceInstance">
		/// The service instance implementing that interface.
		/// </param>
		void AddService(Type serviceInterface, object serviceInstance);
	}
	#endregion
	
	#region IVisualDesignService
	/// <summary>
	/// A service that can visualize non-UIElement objects on a design surface.
	/// </summary>
	public interface IVisualDesignService
	{
		/// <summary>
		/// Create an UIElement for visualizing the object specified by the design site.
		/// </summary>
		/// <returns>
		/// Returns either an UIElement instance that also implements
		/// <see cref="IVisualDesignObjectWrapper"/>,
		/// or returns null to use the component itself as UIElement.
		/// </returns>
		UIElement CreateWrapper(DesignItem site);
	}
	
	/// <summary>
	/// Interface used by the <see cref="IVisualDesignService"/> for UIElements that
	/// are inside the design surfaced but used only to represent a non-UIElement entity.
	/// </summary>
	public interface IVisualDesignObjectWrapper
	{
		/// <summary>
		/// Gets the design site this object was wrapping.
		/// </summary>
		DesignItem WrappedSite { get; }
	}
	#endregion
	
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
		Remove = 0x1,
		/// <summary>
		/// Replace the selection.
		/// </summary>
		Replace = 0x2
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
		
		// /// <summary>Unregisters a component from usage in the designer.</summary>
		// void UnregisterComponentFromDesigner(DesignSite site);
		
		/// <summary>Event raised whenever a component is registered</summary>
		event EventHandler<DesignItemEventArgs> ComponentRegistered;
		/// <summary>Event raised whenever a component is unregistered</summary>
		event EventHandler<DesignItemEventArgs> ComponentUnregistered;
	}
	#endregion
}
