// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Dialog to choose the display binding to use to open a file.
	/// </summary>
	public sealed partial class OpenWithDialog : Form
	{
		sealed class ListEntry
		{
			internal readonly DisplayBindingDescriptor desc;
			
			public ListEntry(DisplayBindingDescriptor desc)
			{
				this.desc = desc;
			}
			
			public override string ToString()
			{
				return StringParser.Parse(desc.Title);
			}
		}
		
		public OpenWithDialog(ICollection<DisplayBindingDescriptor> displayBindings)
		{
			if (displayBindings == null)
				throw new ArgumentNullException("list");
			
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			foreach (DisplayBindingDescriptor desc in displayBindings) {
				programListBox.Items.Add(new ListEntry(desc));
			}
			if (programListBox.Items.Count != 0) {
				programListBox.SelectedIndex = 0;
			}
		}
		
		/// <summary>
		/// Gets the binding that is selected in the list box.
		/// </summary>
		public DisplayBindingDescriptor SelectedBinding {
			get {
				ListEntry e = programListBox.SelectedItem as ListEntry;
				if (e != null)
					return e.desc;
				else
					return null;
			}
		}
	}
}
