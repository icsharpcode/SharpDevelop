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
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates tool bar items from a location in the addin tree.
	/// </summary>
	/// <attribute name="label" use="optional">
	/// Label of the tool bar item.
	/// </attribute>
	/// <attribute name="icon" use="optional">
	/// Icon of the tool bar item.
	/// </attribute>
	/// <attribute name="type" use="optional" enum="Separator;CheckBox;Item;ComboBox;DropDownButton">
	/// This attribute must be one of these values:
	/// Separator, CheckBox, Item, ComboBox, DropDownButton
	/// </attribute>
	/// <attribute name="loadclasslazy" use="optional">
	/// Only for the type "Item". When set to false, the command class is loaded
	/// immediately instead of the usual lazy-loading.
	/// </attribute>
	/// <attribute name="tooltip" use="optional">
	/// Tooltip of the tool bar item.
	/// </attribute>
	/// <attribute name="class">
	/// Command class that is run when item is clicked; or class that manages
	/// the ComboBox/DropDownButton. Required for everything except "Separator".
	/// </attribute>
	/// <attribute name="shortcut" use="optional">
	/// Shortcut that activates the command (e.g. "Control|S").
	/// </attribute>
	/// <usage>Any toolbar strip paths, e.g. /SharpDevelop/Workbench/ToolBar</usage>
	/// <children childTypes="MenuItem">A drop down button has menu items as sub elements.</children>
	/// <returns>
	/// A ToolStrip* object, depending on the type attribute.
	/// </returns>
	/// <conditions>Conditions are handled by the item, "Exclude" maps to "Visible = false", "Disable" to "Enabled = false"</conditions>
	public class ToolbarItemDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new ToolbarItemDescriptor(args.Parameter, args.Codon, args.BuildSubItems<object>(), args.Conditions);
		}
	}
	
	/// <summary>
	/// Represents a toolbar item. These objects are created by the ToolbarItemDoozer and
	/// then converted into GUI-toolkit-specific objects by the ToolbarService.
	/// </summary>
	public sealed class ToolbarItemDescriptor
	{
		public readonly object Parameter;
		public readonly Codon Codon;
		public readonly IList SubItems;
		public readonly IReadOnlyCollection<ICondition> Conditions;
		
		public ToolbarItemDescriptor(object parameter, Codon codon, IList subItems, IReadOnlyCollection<ICondition> conditions)
		{
			this.Parameter = parameter;
			this.Codon = codon;
			this.SubItems = subItems;
			this.Conditions = conditions;
		}
	}
}
