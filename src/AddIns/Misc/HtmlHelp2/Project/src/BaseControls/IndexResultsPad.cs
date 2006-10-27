// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Windows.Forms;
	
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpServices;

	public class ShowIndexResultsMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor indexResults = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2IndexResultsPad));
			if (indexResults != null) indexResults.BringPadToFront();
		}
	}

	public class HtmlHelp2IndexResultsPad : AbstractPadContent
	{
		ListView listView         = new ListView();
		ColumnHeader title        = new ColumnHeader();
		ColumnHeader location     = new ColumnHeader();

		public override Control Control
		{
			get { return listView; }
		}

		public override void RedrawContent()
		{
			this.SetListViewHeader();
		}

		public ListView IndexResultsListView
		{
			get { return listView; }
		}

		public HtmlHelp2IndexResultsPad()
		{
			this.SetListViewHeader();
			listView.Columns.Add(title);
			listView.Columns.Add(location);

			listView.FullRowSelect     = true;
			listView.Alignment         = ListViewAlignment.Left;
			listView.View              = View.Details;
			listView.Dock              = DockStyle.Fill;
			listView.MultiSelect       = false;
			listView.HideSelection     = false;
			listView.Font              = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			ListViewResize(this,null);

			listView.Resize           += new EventHandler(ListViewResize);
			listView.DoubleClick      += new EventHandler(ListViewDoubleClick);
			listView.ColumnClick      += new ColumnClickEventHandler(ColumnClick);
			listView.CreateControl();
		}

		public void SortLV(int listViewColumn)
		{
			listView.ListViewItemSorter = new ListViewItemComparer(listViewColumn);
			listView.Sort();
		}

		private void SetListViewHeader()
		{
			title.Text     = StringParser.Parse("${res:AddIns.HtmlHelp2.Title}");
			location.Text  = StringParser.Parse("${res:AddIns.HtmlHelp2.Location}");
		}

		private void ListViewResize(object sender, EventArgs e)
		{
			int w          = (listView.Width - 60) / 2;
			title.Width    = w;
			location.Width = w;
		}

		private void ListViewDoubleClick(object sender, EventArgs e)
		{
			ListViewItem lvi = listView.SelectedItems[0];
			if (lvi != null && lvi.Tag != null && lvi.Tag is IHxTopic)
			{
				ShowHelpBrowser.OpenHelpView((IHxTopic)lvi.Tag);
			}
		}

		private void ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.SortLV(e.Column);
		}

		public void CleanUp()
		{
			foreach (ListViewItem lvi in listView.Items)
			{
				if(lvi.Tag != null) { lvi.Tag = null; }
			}

			listView.Items.Clear();
		}

		public void SetStatusMessage(string indexTerm)
		{
			 if (listView.Items.Count > 1)
			 {
				 string text = StringParser.Parse("${res:AddIns.HtmlHelp2.ResultsOfIndexResults}",
			 	                                  new string[,]
			 	                                  {{"0", indexTerm},
			 	                                   {"1", listView.Items.Count.ToString(CultureInfo.InvariantCulture)},
			 	                                   {"2", (listView.Items.Count == 1)?"${res:AddIns.HtmlHelp2.SingleTopic}":"${res:AddIns.HtmlHelp2.MultiTopic}"}}
			 	                                 );

				 StatusBarService.SetMessage(text);
			 }
		}

		#region Sorting
		class ListViewItemComparer : IComparer
		{
			private int col;

			public ListViewItemComparer(int column)
			{
				col = column;
			}

			public int Compare(object x, object y)
			{
				return String.Compare(((ListViewItem)x).SubItems[col].Text,
				                      ((ListViewItem)y).SubItems[col].Text);
			}
		}
		#endregion
	}
}
