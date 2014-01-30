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
using ICSharpCode.SharpDevelop;

namespace AddInScout
{
	public class AddInDetailsPanel : Panel
	{
		ListView addInDetailsListView = new ListView();
		Label addInLabel              = new Label();
		
		public AddInDetailsPanel()
		{
			addInDetailsListView.Dock = DockStyle.Fill;
			addInDetailsListView.GridLines = false;
			addInDetailsListView.View = View.Details;
			addInDetailsListView.MultiSelect = false;
			addInDetailsListView.FullRowSelect = true;
			addInDetailsListView.Activation = ItemActivation.OneClick;
			addInDetailsListView.HeaderStyle = ColumnHeaderStyle.None;
			addInDetailsListView.BorderStyle = BorderStyle.FixedSingle;
			addInDetailsListView.ItemActivate += new EventHandler(AddInDetailsListViewItemActivate);
			addInDetailsListView.Columns.Add("Property",100, HorizontalAlignment.Left);
			addInDetailsListView.Columns.Add("Value", 500, HorizontalAlignment.Left);
			Controls.Add(addInDetailsListView);
			
			addInLabel.Dock =DockStyle.Top;
			addInLabel.Text = "AddIn : ";
			addInLabel.Font = new Font(addInLabel.Font.FontFamily,addInLabel.Font.Size*2);
			addInLabel.Height = addInLabel.Height*2;
			addInLabel.FlatStyle = FlatStyle.Flat;
			addInLabel.TextAlign = ContentAlignment.MiddleLeft;
			addInLabel.BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(addInLabel);
		}
		
		void AddInDetailsListViewItemActivate(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			ListViewItem selectedItem = ((ListView)sender).SelectedItems[0];
			
			if (selectedItem.Text.Equals("url", StringComparison.OrdinalIgnoreCase)) {
				string url = selectedItem.SubItems[1].Text;
				try	 {
					System.Diagnostics.Process.Start(url);
				} catch (Exception) {
					// Silent: On my System the browser starts but Process.Start throws an exception. Mike 2.11.2004/Notebook/ICE 1517 on the way to DevCon Europe 2004
//					MessageBox.Show("Unable to Start Browser\n" + ex.ToString());
				}
			} else if (selectedItem.Text.Equals("filename", StringComparison.OrdinalIgnoreCase)) {
				
				FileService.OpenFile(selectedItem.SubItems[1].Text);
			}
			
			Cursor.Current = Cursors.Default;
		}
		
		public void ShowAddInDetails(AddIn ai)
		{
			addInLabel.Text = "AddIn : " + ai.Properties["name"];
			
			addInDetailsListView.Items.Clear();
			
			ListViewItem[] items = new ListViewItem[] {
				new ListViewItem(new string[] { "Author", ai.Properties["author"] }),
				new ListViewItem(new string[] { "Copyright", ai.Properties["copyright"]}),
				new ListViewItem(new string[] { "Description", ai.Properties["description"] }),
				new ListViewItem(new string[] { "FileName", ai.FileName}),
				new ListViewItem(new string[] { "Url", ai.Properties["url"]})
			};
			
			// set Filename & Url rows to 'weblink' style
			items[3].Font = items[4].Font = new Font(addInDetailsListView.Font, FontStyle.Underline);
			items[3].ForeColor = items[4].ForeColor = Color.Blue;
			addInDetailsListView.Items.AddRange(items);
			
			if (ai.Version != null)
				addInDetailsListView.Items.Add(new ListViewItem(new string[] { "Version", ai.Version.ToString()}));
			
			foreach (KeyValuePair<string, Version> entry in ai.Manifest.Identities) {
				ListViewItem newListViewItem = new ListViewItem("Identity");
				newListViewItem.SubItems.Add(entry.Key + " = " + entry.Value);
				addInDetailsListView.Items.Add(newListViewItem);
			}
			
			foreach (AddInReference entry in ai.Manifest.Conflicts) {
				ListViewItem newListViewItem = new ListViewItem("Conflict");
				newListViewItem.SubItems.Add(entry.ToString());
				addInDetailsListView.Items.Add(newListViewItem);
			}
			
			foreach (AddInReference entry in ai.Manifest.Dependencies) {
				ListViewItem newListViewItem = new ListViewItem("Dependency");
				newListViewItem.SubItems.Add(entry.ToString());
				addInDetailsListView.Items.Add(newListViewItem);
			}
			
			foreach (Runtime runtime in ai.Runtimes) {
				ListViewItem newListViewItem = new ListViewItem("Runtime Library");
				newListViewItem.SubItems.Add(runtime.Assembly);
				addInDetailsListView.Items.Add(newListViewItem);
			}
		}
	}
}
