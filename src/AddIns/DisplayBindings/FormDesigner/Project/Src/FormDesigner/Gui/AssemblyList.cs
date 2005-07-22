// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

// created on 26/01/2003 at 21:23
using System;
using System.Windows.Forms;
using System.Text;
using ICSharpCode.Core;

using MSjogren.GacTool.FusionNative;

namespace ICSharpCode.FormDesigner.Gui
{
	public class AssemblyList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ColumnHeader createdObject1;
		private System.Windows.Forms.ColumnHeader createdObject0;
		private System.Windows.Forms.ListView listView;
		
		public AssemblyList()
		{
			InitializeComponents2();
			
			
			okButton.Text = ResourceService.GetString("Global.OKButtonText");
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			
			ColumnHeader referenceHeader;
			
			referenceHeader = listView.Columns[0];
			referenceHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader");
			referenceHeader.Width = 160;
			
			referenceHeader = listView.Columns[1];
			referenceHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader");
			referenceHeader.Width = 70;
						
			//ItemActivate += new EventHandler(AddReference);
			PrintCache();						
		}
		
		void InitializeComponents2()
		{
			// 
			//  Set up generated class form
			// 
			
			
			this.SuspendLayout();
			this.Name = "form";
			this.ShowInTaskbar = false;
			this.Size = new System.Drawing.Size(352, 288);
			this.MinimizeBox = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = FormStartPosition.CenterParent;
			this.TopMost = false;
			this.MaximizeBox = false;
			this.Icon = null;
			this.Text = "Add Assembly";//ResourceService.GetString("Dialog.Options.FormDesigner.AddAssembly");
			
			// 
			//  Set up member listView
			// 
			listView = new System.Windows.Forms.ListView();
			listView.Name = "listView";
			listView.Dock = System.Windows.Forms.DockStyle.Top;
			listView.TabIndex = 3;
			listView.View = System.Windows.Forms.View.Details;
			listView.Size = new System.Drawing.Size(344, 216);
			listView.CheckBoxes = true;
			listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			
			// 
			//  Set up member createdObject0
			// 
			createdObject0 = new System.Windows.Forms.ColumnHeader();
			createdObject0.Text = "reference";
			listView.Columns.Add(createdObject0);
			
			// 
			//  Set up member createdObject1
			// 
			createdObject1 = new System.Windows.Forms.ColumnHeader();
			createdObject1.Text = "version";
			listView.Columns.Add(createdObject1);
			this.Controls.Add(listView);
			
			// 
			//  Set up member CancelButton
			// 
			cancelButton = new System.Windows.Forms.Button();
			cancelButton.Name = "CancelButton";
			cancelButton.Location = new System.Drawing.Point(264, 224);
			cancelButton.Text = "button2";
			cancelButton.TabIndex = 2;
			cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Controls.Add(cancelButton);
			
			// 
			//  Set up member okButton
			// 
			okButton = new System.Windows.Forms.Button();
			okButton.Name = "okButton";
			okButton.Location = new System.Drawing.Point(184, 224);
			okButton.Text = "button";
			okButton.TabIndex = 1;
			okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Controls.Add(okButton);
			this.ResumeLayout(false);
		}

		void PrintCache()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
				
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
									
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string[] info = sb.ToString().Split(',');
				
				string aName    = info[0];
				string aVersion = info[1].Substring(info[1].LastIndexOf('=') + 1);
				ListViewItem item = new ListViewItem(new string[] {aName, aVersion});
				item.Tag = sb.ToString();
				listView.Items.Add(item);
			}
		}
		
		public ListView.CheckedListViewItemCollection GetCheckedItems()
		{
			return listView.CheckedItems;
		}
		
	}
}
