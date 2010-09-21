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
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string label = codon.Properties["label"];
			
			if (subItems == null || subItems.Count == 0) {
				if (codon.Properties.Contains("class")) {
					return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label), codon.AddIn, codon.Properties["class"]);
				} else {
					return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label));
				}
			}
			
			List<IDialogPanelDescriptor> newList = new List<IDialogPanelDescriptor>();
			foreach (IDialogPanelDescriptor d in subItems) {
				newList.Add(d);
			}
			
			return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label), newList);
		}
	}
}
