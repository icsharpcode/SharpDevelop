// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class AttachToProcessForm : Form
	{
		public AttachToProcessForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			UpdateResourceStrings();
			RefreshProcessList();
		}
		
		public System.Diagnostics.Process Process {
			get { 
				return GetSelectedProcess();
			}
		}	
		
		void RefreshProcessList()
		{
			listView.Items.Clear();
			foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses()) {
				try {
					string fileName = Path.GetFileName(process.MainModule.FileName);
					ListViewItem item = new ListViewItem(fileName);
					item.SubItems.Add(process.Id.ToString());
					item.SubItems.Add(process.MainWindowTitle);
					item.Tag = process;
					listView.Items.Add(item);
				} catch (Win32Exception) {
					// Do nothing.
				}
			}
			
			listView.Sort();
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
	}
}
