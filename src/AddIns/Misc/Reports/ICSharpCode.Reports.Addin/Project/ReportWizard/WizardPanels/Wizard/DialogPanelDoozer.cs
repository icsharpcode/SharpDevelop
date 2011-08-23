// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Creates DefaultDialogPanelDescriptor objects that are used in option dialogs.
	/// </summary>
	/// <attribute name="class">
	/// Name of the IDialogPanel class. Optional if the page has subpages.
	/// </attribute>
	/// <attribute name="label" use="required">
	/// Caption of the dialog panel.
	/// </attribute>
	/// <children childTypes="DialogPanel">
	/// In the SharpDevelop options, option pages can have subpages by specifying them
	/// as children in the AddInTree.
	/// </children>
	/// <returns>
	/// A DefaultDialogPanelDescriptor object.
	/// </returns>
	public class DialogPanelDoozer : IDoozer
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
			string label = args.Codon["label"];
			string id = args.Codon.Id;
			
			var subItems = args.BuildSubItems<IDialogPanelDescriptor>();
			if (subItems.Count == 0) {
				if (args.Codon.Properties.Contains("class")) {
					return new DefaultDialogPanelDescriptor(id, StringParser.Parse(label), args.AddIn, args.Codon["class"]);
				} else {
					return new DefaultDialogPanelDescriptor(id, StringParser.Parse(label));
				}
			}
			
			return new DefaultDialogPanelDescriptor(id, StringParser.Parse(label), subItems);
		}
	}
}
