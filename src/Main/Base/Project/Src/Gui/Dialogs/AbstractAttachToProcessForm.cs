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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class AbstractAttachToProcessForm : Form
	{		
		public AbstractAttachToProcessForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			UpdateResourceStrings();
			RefreshProcessList();
		}
		
		public System.Diagnostics.Process Process {
			get { return GetSelectedProcess(); }
		}	

		protected virtual void RefreshProcessList(ListView listView, bool showNonManaged)
		{
		}
		
		void RefreshProcessList()
		{
			RefreshProcessList(listView, showNonManagedCheckBox.Checked);
			
			if (listView.Items.Count > 0) {
				listView.Items[0].Selected = true;
			} else {
				attachButton.Enabled = false;
			}
		}
		
		System.Diagnostics.Process GetSelectedProcess()
		{
			if (listView.SelectedItems.Count > 0) {
				return listView.SelectedItems[0].Tag as System.Diagnostics.Process;
			}
			return null;
		}
		
		void RefreshButtonClick(object sender, EventArgs e)
		{
			RefreshProcessList();
		}
		
		void UpdateResourceStrings()
		{
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AttachToProcessForm.Title}");
			
			processColumnHeader.Text = StringParser.Parse("${res:ComponentInspector.AttachDialog.ProcessLabel}");
			processIdColumnHeader.Text = StringParser.Parse("${res:Global.ID}");
			titleColumnHeader.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.Title}");
			                 
			attachButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AttachToProcessForm.AttachButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			refreshButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RefreshButtonTooltip}");
		}
		
		void ListViewItemActivate(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
				
		void ShowNonManagedCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			RefreshProcessList();
		}
		
		void ListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			attachButton.Enabled = listView.SelectedItems.Count > 0;
		}
	}
}
