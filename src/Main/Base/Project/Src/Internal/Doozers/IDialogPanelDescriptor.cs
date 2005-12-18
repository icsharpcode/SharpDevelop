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
	public interface IDialogPanelDescriptor
	{
		/// <value>
		/// Returns the ID of the dialog panel codon
		/// </value>
		string ID {
			get;
		}
		
		/// <value>
		/// Returns the label of the dialog panel
		/// </value>
		string Label {
			get;
			set;
		}
		
		/// <summary>
		/// The child dialog panels (e.g. for treeviews)
		/// </summary>
		ArrayList ChildDialogPanelDescriptors {
			get;
			set;
		}
		
		/// <value>
		/// Returns the dialog panel object
		/// </value>
		IDialogPanel DialogPanel {
			get;
			set;
		}
	}
}
