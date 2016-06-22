// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Dialog to choose the display binding to use to open a file.
	/// </summary>
	sealed partial class OpenWithDialog : Form
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
			SD.WinForms.ApplyRightToLeftConverter(this);
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
					programListBox.Items.Add(new ListEntry(SD.DisplayBindingService.AddExternalProcessDisplayBinding(binding), false));
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
		
		void ProgramListBoxDoubleClicked(object sender, EventArgs e)
		{
			okButton.PerformClick();
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			SD.DisplayBindingService.RemoveExternalProcessDisplayBinding((ExternalProcessDisplayBinding)SelectedBinding.GetLoadedBinding());
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
			SD.DisplayBindingService.SetDefaultCodon(fileExtension, SelectedBinding);
		}
	}
}
