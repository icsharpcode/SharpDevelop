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
using System.Linq;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates menu items from a location in the addin tree.
	/// </summary>
	/// <attribute name="label" use="required">
	/// Label of the menu item.
	/// </attribute>
	/// <attribute name="type" use="optional" enum="Separator;CheckBox;Item;Command;Menu;Builder">
	/// This attribute must be one of these values:
	/// Separator, CheckBox, Item=Command, Menu (=with subitems),
	/// Builder (=class implementing ISubmenuBuilder).
	/// Default: Command.
	/// </attribute>
	/// <attribute name="loadclasslazy" use="optional">
	/// Only for the type "Item"/"Command".
	/// When set to false, the command class is loaded
	/// immediately instead of the usual lazy-loading.
	/// </attribute>
	/// <attribute name="icon" use="optional">
	/// Icon of the menu item.
	/// </attribute>
	/// <attribute name="class" use="optional">
	/// Command class that is run when item is clicked.
	/// </attribute>
	/// <attribute name="command" use="optional">
	/// A WPF routed command that is executed when item is clicked.
	/// Currently, this property is supported only for WPF Menus.
	/// Only one of the "class" and "command" attributes can be used on a menu entry.
	/// </attribute>
	/// <attribute name="link" use="optional">
	/// Only for the type "Item"/"Command". Opens a webpage instead of running a command when
	/// clicking the item.
	/// </attribute>
	/// <attribute name="shortcut" use="optional">
	/// Shortcut that activates the command (e.g. "Control|S").
	/// </attribute>
	/// <children childTypes="MenuItem">
	/// If "type" is "Menu", the item can have sub-menuitems.
	/// </children>
	/// <usage>Any menu strip paths or context menu paths, e.g. /SharpDevelop/Workbench/MainMenu</usage>
	/// <returns>
	/// A MenuItemDescriptor object.
	/// </returns>
	/// <conditions>Conditions are handled by the item, "Exclude" maps to "Visible = false", "Disable" to "Enabled = false"</conditions>
	public class MenuItemDoozer : IDoozer
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
			return new MenuItemDescriptor(args.Parameter, args.Codon, args.BuildSubItems<object>(), args.Conditions);
		}
	}
	
	/// <summary>
	/// Represents a menu item. These objects are created by the MenuItemDoozer and
	/// then converted into GUI-toolkit-specific objects by the MenuService.
	/// </summary>
	public sealed class MenuItemDescriptor
	{
		public readonly object Parameter;
		public readonly Codon Codon;
		public readonly IList SubItems;
		public readonly IReadOnlyCollection<ICondition> Conditions;
		
		public MenuItemDescriptor(object parameter, Codon codon, IList subItems, IReadOnlyCollection<ICondition> conditions)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.Parameter = parameter;
			this.Codon = codon;
			this.SubItems = subItems;
			this.Conditions = conditions;
		}
	}
}
