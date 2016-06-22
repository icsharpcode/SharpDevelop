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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
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
	sealed class DisplayBindingDoozer : IDoozer
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
