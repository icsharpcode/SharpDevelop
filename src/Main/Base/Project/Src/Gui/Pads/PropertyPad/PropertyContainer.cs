// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.SharpDevelop.Gui
{
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
		}
	}
}
