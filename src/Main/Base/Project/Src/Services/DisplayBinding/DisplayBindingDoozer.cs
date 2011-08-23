// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates DisplayBindingDescriptor objects.
	/// Primary display bindings can provide editors for additional file types
	/// (like the ResourceEditor), secondary display bindings can add tabs to
	/// existing display bindings (like the form designer).
	/// </summary>
	/// <attribute name="class" use="required">
	/// Name of the IDisplayBinding or ISecondaryDisplayBinding class.
	/// </attribute>
	/// <attribute name="title" use="required">
	/// Title of the display binding to use in the "Open With" dialog.
	/// </attribute>
	/// <attribute name="type" use="optional" enum="Primary;Secondary">
	/// Type of the display binding (either "Primary" or "Secondary"). Default: "Primary".
	/// </attribute>
	/// <attribute name="fileNamePattern" use="optional">
	/// Regular expression that specifies the file names for which the display binding
	/// will be used. Example: "\.res(x|ources)$"
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/DisplayBindings</usage>
	/// <returns>
	/// An DisplayBindingDescriptor object that wraps either a IDisplayBinding
	/// or a ISecondaryDisplayBinding object.
	/// </returns>
	/// <example title="Primary display binding: Resource editor">
	/// &lt;Path name = "/SharpDevelop/Workbench/DisplayBindings"&gt;
	///   &lt;DisplayBinding id    = "ResourceEditor"
	///                   title = "Resource editor"
	///                   class = "ResourceEditor.ResourceEditorDisplayBinding"
	///                   insertbefore    = "Text"
	///                   fileNamePattern = "\.res(x|ources)$"/&gt;
	/// &lt;/Path&gt;
	/// </example>
	/// <example title="Secondary display binding: Form designer">
	/// &lt;Path name = "/SharpDevelop/Workbench/DisplayBindings"&gt;
	///   &lt;DisplayBinding id  = "FormsDesigner"
	///                   title = "Windows Forms Designer"
	///                   type  = "Secondary"
	///                   class = "ICSharpCode.FormsDesigner.FormsDesignerSecondaryDisplayBinding"
	///                   fileNamePattern = "\.(cs|vb)$" /&gt;
	/// &lt;/Path&gt;
	/// </example>
	public class DisplayBindingDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(BuildItemArgs args)
		{
			return new DisplayBindingDescriptor(args.Codon);
		}
	}
}
