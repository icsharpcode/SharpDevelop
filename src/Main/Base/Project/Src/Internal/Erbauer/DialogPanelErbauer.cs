// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;

namespace ICSharpCode.Core
{
	public class DialogPanelErbauer : IErbauer
	{
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string label = codon.Properties["label"];
			
			if (subItems == null || subItems.Count == 0) {				
				if (codon.Properties.Contains("class")) {
					return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label), (IDialogPanel)codon.AddIn.CreateObject(codon.Properties["class"]));
				} else {
					return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label));
				}
			}
			
			return new DefaultDialogPanelDescriptor(codon.Id, StringParser.Parse(label), subItems);
		}
	}
}
