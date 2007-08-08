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
		
		string fileExtension;
		
		public OpenWithDialog(ICollection<DisplayBindingDescriptor> displayBindings, string fileExtension)
		{
			if (displayBindings == null)
				throw new ArgumentNullException("list");
			
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			this.fileExtension = fileExtension;
			if (string.IsNullOrEmpty(fileExtension))
				addButton.Enabled = false;
			
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
		
		void AddButtonClick(object sender, EventArgs e)
		{
			using (AddOpenWithEntryDialog dlg = new AddOpenWithEntryDialog()) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					ExternalProcessDisplayBinding binding = new ExternalProcessDisplayBinding {
						FileExtension = fileExtension,
						CommandLine = dlg.ProgramName,
						Title = dlg.DisplayName,
						Id = Guid.NewGuid().ToString()
					};
					programListBox.Items.Add(new ListEntry(DisplayBindingService.AddExternalProcessDisplayBinding(binding)));
				}
			}
		}
		
		void ProgramListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			DisplayBindingDescriptor binding = SelectedBinding;
			if (binding != null) {
				okButton.Enabled = true;
				removeButton.Enabled = binding.GetLoadedBinding() is ExternalProcessDisplayBinding;
			} else {
				okButton.Enabled = false;
				removeButton.Enabled = false;
			}
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			DisplayBindingService.RemoveExternalProcessDisplayBinding((ExternalProcessDisplayBinding)SelectedBinding.GetLoadedBinding());
			programListBox.Items.RemoveAt(programListBox.SelectedIndex);
		}
	}
}
