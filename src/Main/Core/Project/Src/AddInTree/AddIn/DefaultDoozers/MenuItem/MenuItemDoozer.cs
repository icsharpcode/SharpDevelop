// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections;

namespace ICSharpCode.Core
{
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
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new MenuSeparator(codon, caller);
				case "CheckBox":
					return new MenuCheckBox(codon, caller);
				case "Item":
					return new MenuCommand(codon, caller, createCommand);
				case "Command":
					return new MenuCommand(codon, caller, createCommand);
				case "Menu":
					return new Menu(codon, caller, subItems);
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
	}
}
