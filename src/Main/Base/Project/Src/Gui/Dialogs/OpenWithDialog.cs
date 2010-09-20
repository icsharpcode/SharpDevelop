// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

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
			internal readonly bool IsDefault;
			
			public ListEntry(DisplayBindingDescriptor desc, bool isDefault)
			{
				this.desc = desc;
				this.IsDefault = isDefault;
			}
			
			public override string ToString()
			{
				if (IsDefault)
					return StringParser.Parse(desc.Title + " (${res:Gui.ProjectBrowser.OpenWith.Default})");
				else
					return StringParser.Parse(desc.Title);
			}
		}
		
		string fileExtension;
		int defaultBindingIndex;
		
		public OpenWithDialog(IList<DisplayBindingDescriptor> displayBindings, int defaultBindingIndex, string fileExtension)
		{
			if (displayBindings == null)
				throw new ArgumentNullException("list");
			
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			this.Text = StringParser.Parse(this.Text);
			
			this.fileExtension = fileExtension;
			this.defaultBindingIndex = defaultBindingIndex;
			if (string.IsNullOrEmpty(fileExtension))
				addButton.Enabled = false;
			
			for (int i = 0; i < displayBindings.Count; i++) {
				programListBox.Items.Add(new ListEntry(displayBindings[i], i == defaultBindingIndex));
			}
			if (defaultBindingIndex < programListBox.Items.Count) {
				programListBox.SelectedIndex = defaultBindingIndex;
			}
			RightToLeftConverter.ConvertRecursive(this);
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
					programListBox.Items.Add(new ListEntry(DisplayBindingService.AddExternalProcessDisplayBinding(binding), false));
				}
			}
		}
		
		void ProgramListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			DisplayBindingDescriptor binding = SelectedBinding;
			if (binding != null) {
				okButton.Enabled = true;
				setAsDefaultButton.Enabled = true;
				removeButton.Enabled = binding.GetLoadedBinding() is ExternalProcessDisplayBinding;
			} else {
				okButton.Enabled = false;
				setAsDefaultButton.Enabled = false;
				removeButton.Enabled = false;
			}
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			DisplayBindingService.RemoveExternalProcessDisplayBinding((ExternalProcessDisplayBinding)SelectedBinding.GetLoadedBinding());
			if (defaultBindingIndex == programListBox.SelectedIndex)
				defaultBindingIndex = -1;
			programListBox.Items.RemoveAt(programListBox.SelectedIndex);
		}
		
		void SetAsDefaultButtonClick(object sender, EventArgs e)
		{
			if (defaultBindingIndex >= 0) {
				programListBox.Items[defaultBindingIndex] = new ListEntry(
					((ListEntry)programListBox.Items[defaultBindingIndex]).desc,
					false);
			}
			defaultBindingIndex = programListBox.SelectedIndex;
			programListBox.Items[defaultBindingIndex] = new ListEntry(
				((ListEntry)programListBox.Items[defaultBindingIndex]).desc,
				true);
			DisplayBindingService.SetDefaultCodon(fileExtension, SelectedBinding);
		}
	}
}
