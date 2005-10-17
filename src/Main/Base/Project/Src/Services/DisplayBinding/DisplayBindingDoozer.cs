// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.Core;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates DisplayBindingDescriptor objects.
	/// </summary>
	/// <attribute name="class">
	/// Name of the IDisplayBinding or ISecondaryDisplayBinding class.
	/// </attribute>
	/// <attribute name="type">
	/// Type of the display binding (either "Primary" or "Secondary"). Default: "Primary".
	/// </attribute>
	/// <attribute name="fileNamePattern">
	/// Optional. Regular expression that specifies the file names for which the display binding
	/// will be used. Example: "\.res(x|ources)$"
	/// </attribute>
	/// <attribute name="languagePattern">
	/// Optional. Regular expression that specifies the language for which the display binding
	/// will be used. Example: "\Resource Files$"
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/DisplayBindings</usage>
	/// <returns>
	/// An DisplayBindingDescriptor object that wraps either a IDisplayBinding
	/// or a ISecondaryDisplayBinding object.
	/// </returns>
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
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
//			if (subItems == null || subItems.Count > 0) {
//				throw new ApplicationException("Tried to buil a command with sub commands, please check the XML definition.");
//			}
			return new DisplayBindingDescriptor(codon);
		}
	}
}
