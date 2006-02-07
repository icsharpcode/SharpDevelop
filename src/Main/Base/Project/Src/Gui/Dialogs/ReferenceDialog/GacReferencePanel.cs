// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GacReferencePanel : UserControl, IReferencePanel
	{
		class ColumnSorter : System.Collections.IComparer
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

		protected ListView listView;
		CheckBox chooseSpecificVersionCheckBox;
		ISelectReferenceDialog selectDialog;
		ColumnSorter sorter;
		
		public GacReferencePanel(ISelectReferenceDialog selectDialog)
		{
			listView = new ListView();
			sorter = new ColumnSorter();
			listView.ListViewItemSorter = sorter;
			
			this.selectDialog = selectDialog;
			
			ColumnHeader referenceHeader = new ColumnHeader();
			referenceHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader");
			referenceHeader.Width = 180;
			listView.Columns.Add(referenceHeader);
			
			listView.Sorting = SortOrder.Ascending;
			
			ColumnHeader versionHeader = new ColumnHeader();
			versionHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader");
			versionHeader.Width = 70;
			listView.Columns.Add(versionHeader);
			
			ColumnHeader pathHeader = new ColumnHeader();
			pathHeader.Text  = ResourceService.GetString("Global.Path");
			pathHeader.Width = 100;
			listView.Columns.Add(pathHeader);
			
			listView.View = View.Details;
			listView.FullRowSelect = true;
			listView.ItemActivate += delegate { AddReference(); };
			listView.ColumnClick += new ColumnClickEventHandler(columnClick);
			
			listView.Dock = DockStyle.Fill;
			this.Dock = DockStyle.Fill;
			this.Controls.Add(listView);
			
			chooseSpecificVersionCheckBox = new CheckBox();
			chooseSpecificVersionCheckBox.Dock = DockStyle.Top;
			chooseSpecificVersionCheckBox.Text = StringParser.Parse("${res:Dialog.SelectReferenceDialog.GacReferencePanel.ChooseSpecificAssemblyVersion}");
			this.Controls.Add(chooseSpecificVersionCheckBox);
			chooseSpecificVersionCheckBox.CheckedChanged += delegate {
				listView.Items.Clear();
				if (chooseSpecificVersionCheckBox.Checked)
					listView.Items.AddRange(fullItemList);
				else
					listView.Items.AddRange(shortItemList);
			};
			
			PrintCache();
		}
		
		void columnClick(object sender, ColumnClickEventArgs e)
		{
			if(e.Column < 2) {
				sorter.CurrentColumn = e.Column;
				listView.Sort();
			}
		}
		
		public void AddReference()
		{
			foreach (ListViewItem item in listView.SelectedItems) {
				selectDialog.AddReference(ReferenceType.Gac,
				                          item.Text,
				                          chooseSpecificVersionCheckBox.Checked ? item.Tag.ToString() : item.Text,
				                          null);
			}
		}
		
		ListViewItem[] fullItemList;
		/// <summary>
		/// Item list where older versions are filtered out.
		/// </summary>
		ListViewItem[] shortItemList;
		
		void PrintCache()
		{
			List<ListViewItem> itemList = GetCacheContent();
			
			fullItemList = itemList.ToArray();
			// Remove all items where a higher version exists
			itemList.RemoveAll(delegate(ListViewItem item) {
			                   	return itemList.Exists(delegate(ListViewItem item2) {
			                   	                       	return string.Equals(item.Text, item2.Text, StringComparison.OrdinalIgnoreCase)
			                   	                       		&& new Version(item.SubItems[1].Text) < new Version(item2.SubItems[1].Text);
			                   	                       });
			                   });
			shortItemList = itemList.ToArray();
			
			listView.Items.AddRange(shortItemList);
		}
		
		protected virtual List<ListViewItem> GetCacheContent()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			List<ListViewItem> itemList = new List<ListViewItem>();
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
				itemList.Add(item);
			}
			return itemList;
		}
	}
}
