// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

/* TODO : Dirty Files dialog 
* see DefaultWorkbench.cs OnClosing method
using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

using ICSharpCode.Core.Gui;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class DirtyFilesDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Button savebutton;
		private System.Windows.Forms.Button saveallbutton;
		private System.Windows.Forms.Button discardallbutton;
		private System.Windows.Forms.Button cancelbutton;
		
		public DirtyFilesDialog()
		{
			InitializeComponent();
			
			listView1.Columns.Add(ResourceService.GetString("Dialog.DirtyFiles.Files"), listView1.Width - 5, HorizontalAlignment.Left);
			
			listView1.SmallImageList = FileUtility.ImageList;
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsDirty && content.FileName != null) {
					ListViewItem item = new ListViewItem(content.FileName, FileUtility.GetImageIndexFor(content.FileName));
					item.Selected = true;
					listView1.Items.Add(item);
				}
			}
		}
		
		void DiscardAll(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
		
		void SaveAll(object sender, EventArgs e)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsDirty && content.FileName != null) {
					content.SaveFile();
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void SaveSelected(object sender, EventArgs e)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsDirty && content.FileName != null) {
					foreach (ListViewItem item in listView1.SelectedItems) {
						if (item.Text == content.FileName) {
							content.SaveFile();
							break;
						}
					}
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		private void InitializeComponent()
		{
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)PropertyService.Get("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.savebutton = new System.Windows.Forms.Button();
			this.saveallbutton = new System.Windows.Forms.Button();
			this.discardallbutton = new System.Windows.Forms.Button();
			this.cancelbutton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			//
			// groupBox1
			//
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {this.listView1});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(312, 272);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = ResourceService.GetString("Dialog.DirtyFiles.DirtyFiles");
			
			//
			// listView1
			//
			this.listView1.HideSelection = false;
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView1.Location = new System.Drawing.Point(8, 16);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(296, 248);
			this.listView1.TabIndex = 0;
			this.listView1.View = System.Windows.Forms.View.Details;
			listView1.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			//
			// savebutton
			//
			this.savebutton.Location = new System.Drawing.Point(328, 16);
			this.savebutton.Name = "savebutton";
			this.savebutton.TabIndex = 1;
			this.savebutton.Size = new System.Drawing.Size(96, 24);
			this.savebutton.Text = ResourceService.GetString("Dialog.DirtyFiles.SaveButton");
			
			this.savebutton.Click += new EventHandler(SaveSelected);
			savebutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// saveallbutton
			//
			this.saveallbutton.Location = new System.Drawing.Point(328, 48);
			this.saveallbutton.Name = "saveallbutton";
			this.saveallbutton.TabIndex = 2;
			this.saveallbutton.Size = new System.Drawing.Size(96, 24);
			this.saveallbutton.Text = ResourceService.GetString("Dialog.DirtyFiles.SaveAllButton");
			
			this.saveallbutton.Click += new EventHandler(SaveAll);
			saveallbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// discardallbutton
			//
			this.discardallbutton.Location = new System.Drawing.Point(328, 80);
			this.discardallbutton.Name = "discardallbutton";
			this.discardallbutton.TabIndex = 3;
			this.discardallbutton.Size = new System.Drawing.Size(96, 24);
			this.discardallbutton.Text = ResourceService.GetString("Dialog.DirtyFiles.DiscardAllButton");
			
			this.discardallbutton.Click += new EventHandler(DiscardAll);
			discardallbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// cancelbutton
			//
			this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelbutton.Location = new System.Drawing.Point(328, 112);
			this.cancelbutton.Name = "cancelbutton";
			this.cancelbutton.TabIndex = 4;
			this.cancelbutton.Size = new System.Drawing.Size(96, 24);
			this.cancelbutton.Text = ResourceService.GetString("Global.CancelButtonText");
			cancelbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// Win32Form1
			//
			this.AcceptButton = this.savebutton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelbutton;
			this.ClientSize = new System.Drawing.Size(430, 290);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.cancelbutton,
			                       this.discardallbutton,
			                       this.saveallbutton,
			                       this.savebutton,
			                       this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = ResourceService.GetString("Dialog.DirtyFiles.DialogName");
			
			this.TopMost = true;
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
}
*/
