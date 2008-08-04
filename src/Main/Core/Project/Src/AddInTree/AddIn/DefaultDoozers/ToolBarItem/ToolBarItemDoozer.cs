// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

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
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new ToolbarItemDescriptor(caller, codon, subItems);
		}
	}
	
	/// <summary>
	/// Represents a toolbar item. These objects are created by the ToolbarItemDoozer and
	/// then converted into GUI-toolkit-specific objects by the ToolbarService.
	/// </summary>
	public sealed class ToolbarItemDescriptor
	{
		public readonly object Caller;
		public readonly Codon Codon;
		public readonly IList SubItems;
		
		public ToolbarItemDescriptor(object caller, Codon codon, IList subItems)
		{
			this.Caller = caller;
			this.Codon = codon;
			this.SubItems = subItems;
		}
	}
}
