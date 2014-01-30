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
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A IViewContent or IPadContent can implement this interface to display a set of properties in
	/// the property grid when it has focus.
	/// One view/pad content instance has to always return the same property container instance
	/// and has to change only the properties on that PropertyContainer.
	/// </summary>
	[ViewContentService]
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
			if (createPadOnConstruction) {
				PadDescriptor desc = SD.Workbench.GetPad(typeof(PropertyPad));
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
