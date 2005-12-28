// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GacReferencePanel : ListView, IReferencePanel
	{
		
		class ColumnSorter : IComparer
		{
			private int column = 0;
			bool asc = true;
			
			public int CurrentColumn
			{
				get
				{
					return column;
				}
				set
				{
					if(column == value) asc = !asc;
					else column = value;
				}
			}
			
			public int Compare(object x, object y)
			{
				ListViewItem rowA = (ListViewItem)x;
				ListViewItem rowB = (ListViewItem)y;
				int result = String.Compare(rowA.SubItems[CurrentColumn].Text, rowB.SubItems[CurrentColumn].Text);
				if(asc) return result;
				else return result * -1;
			}
		}

		SelectReferenceDialog selectDialog;
		ColumnSorter sorter;
		
		public GacReferencePanel(SelectReferenceDialog selectDialog)
		{
			sorter = new ColumnSorter();
			this.ListViewItemSorter = sorter;
			
			this.selectDialog = selectDialog;
			
			ColumnHeader referenceHeader = new ColumnHeader();
			referenceHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader");
			referenceHeader.Width = 160;
			Columns.Add(referenceHeader);
			
			Sorting = SortOrder.Ascending;
			
			ColumnHeader versionHeader = new ColumnHeader();
			versionHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader");
			versionHeader.Width = 70;
			Columns.Add(versionHeader);
			
			ColumnHeader pathHeader = new ColumnHeader();
			pathHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.PathHeader");
			pathHeader.Width = 100;
			Columns.Add(pathHeader);
			
			View = View.Details;
			Dock = DockStyle.Fill;
			FullRowSelect = true;
			ItemActivate += new EventHandler(AddReference);
			ColumnClick += new ColumnClickEventHandler(columnClick);
			PrintCache();
		}
		
		void columnClick(object sender, ColumnClickEventArgs e)
		{
			if(e.Column < 2)
			{
				sorter.CurrentColumn = e.Column;
				Sort();
			}
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			foreach (ListViewItem item in SelectedItems) {
				selectDialog.AddReference(ReferenceType.Gac,
				                          item.Text,
				                          item.Tag.ToString(),
				                          null);
			}
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
				Items.Add(item);
			}
		}
	}
}
