// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	public interface IOptionPanelDescriptor
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
		IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors {
			get;
		}
		
		/// <value>
		/// Returns the dialog panel object
		/// </value>
		IOptionPanel OptionPanel {
			get;
		}
		
		/// <summary>
		/// Gets whether the descriptor has an option panel (as opposed to having only child option panels)
		/// </summary>
		bool HasOptionPanel {
			get;
		}
	}
}
