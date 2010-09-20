// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A IViewContent or IPadContent can implement this interface to display a set of properties in
	/// the property grid when it has focus.
	/// One view/pad content instance has to always return the same property container instance
	/// and has to change only the properties on that PropertyContainer.
	/// </summary>
	public interface IHasPropertyContainer
	{
		PropertyContainer PropertyContainer { get; }
	}
	
	/// <summary>
	/// A PropertyContainer is a little helper class that combines the settings a ViewContent
	/// can set on the PropertyGrid.
	/// Every pad or view content can have a property container associated with it by implementing
	/// IHasPropertyContainer.
	/// The PropertyPad follows the focus and displays the properties for the last pad that
	/// has specified a PropertyContainer.
	/// Changing properties on the container automatically updates the pad if the property container
	/// is currently displayed there.
	/// </summary>
	public sealed class PropertyContainer
	{
		/// <summary>
		/// Creates a new PropertyContainer instance.
		/// This has the side effect of constructing the PropertyPad if necessary.
		/// </summary>
		public PropertyContainer() : this(true) { }
		
		internal PropertyContainer(bool createPadOnConstruction)
		{
			if (createPadOnConstruction && WorkbenchSingleton.Workbench != null) {
				PadDescriptor desc = WorkbenchSingleton.Workbench.GetPad(typeof(PropertyPad));
				if (desc != null) desc.CreatePad();
			}
		}
		
		/// <summary>
		/// Gets if this property container is currently shown in the property grid.
		/// </summary>
		public bool IsActivePropertyContainer {
			get { return PropertyPad.ActiveContainer == this; }
		}
		
		object selectedObject;
		object[] selectedObjects;
		
		public object SelectedObject {
			get {
				return selectedObject;
			}
			set {
				selectedObject = value;
				selectedObjects = null;
				PropertyPad.UpdateSelectedObjectIfActive(this);
			}
		}
		
		public object[] SelectedObjects {
			get {
				return selectedObjects;
			}
			set {
				selectedObject = null;
				selectedObjects = value;
				PropertyPad.UpdateSelectedObjectIfActive(this);
			}
		}
		
		ICollection selectableObjects;
		
		public ICollection SelectableObjects {
			get {
				return selectableObjects;
			}
			set {
				selectableObjects = value;
				PropertyPad.UpdateSelectableIfActive(this);
			}
		}
		
		IDesignerHost host;
		
		public IDesignerHost Host {
			get {
				return host;
			}
			set {
				host = value;
				PropertyPad.UpdateHostIfActive(this);
			}
		}
		
		object propertyGridReplacementContent;
		
		public object PropertyGridReplacementContent {
			get { return propertyGridReplacementContent; }
			set {
				propertyGridReplacementContent = value;
				PropertyPad.UpdatePropertyGridReplacementContent(this);
			}
		}
		
		/// <summary>
		/// Clears all properties on this container.
		/// When a ViewContent is closed, it should call Clear on it's property container to
		/// remove it from the property pad.
		/// </summary>
		public void Clear()
		{
			Host = null;
			SelectableObjects = null;
			SelectedObject = null;
			PropertyGridReplacementContent = null;
		}
	}
}
