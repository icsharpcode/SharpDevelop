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

	public class HtmlHelp2SearchResultsView : UserControl
	{
		ListView listView         = new ListView();
		ColumnHeader title        = new ColumnHeader();
		ColumnHeader location     = new ColumnHeader();
		ColumnHeader rank         = new ColumnHeader();
		
		static HtmlHelp2SearchResultsView instance;

		public static HtmlHelp2SearchResultsView Instance
		{
			get
			{
				if (instance == null) instance = new HtmlHelp2SearchResultsView();
				return instance;
			}
		}
		
		public override void Refresh()
		{
			this.SetListViewHeader();
			base.Refresh();
		}

		public ListView SearchResultsListView
		{
			get { return listView; }
		}

		public HtmlHelp2SearchResultsView()
		{
			this.SetListViewHeader();
			listView.Columns.Add(title);
			listView.Columns.Add(location);
			listView.Columns.Add(rank);

			listView.FullRowSelect     = true;
			listView.AutoArrange       = true;
			listView.Alignment         = ListViewAlignment.Left;
			listView.View              = View.Details;
			listView.Dock              = DockStyle.Fill;
			listView.Font              = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			ListViewResize(this,null);

			listView.Resize           += new EventHandler(ListViewResize);
			listView.DoubleClick      += new EventHandler(ListViewDoubleClick);
			listView.ColumnClick      += new ColumnClickEventHandler(ColumnClick);
			Controls.Add(listView);
		}

		private void SetListViewHeader()
		{
			title.Text    = StringParser.Parse("${res:AddIns.HtmlHelp2.Title}");
			location.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.Location}");
			rank.Text     = StringParser.Parse("${res:AddIns.HtmlHelp2.Rank}");
		}

		private void ListViewResize(object sender, EventArgs e)
		{
			rank.Width     = 80;
			int w          = (listView.Width - rank.Width - 40) / 2;
			title.Width    = w;
			location.Width = w;
		}

		private void ListViewDoubleClick(object sender, EventArgs e)
		{
			PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
			bool hiliteMatches = (search != null && ((HtmlHelp2SearchPad)search.PadContent).HiliteEnabled);

			ListViewItem lvi = listView.SelectedItems[0];
			if (lvi != null && lvi.Tag != null && lvi.Tag is IHxTopic)
			{
				ShowHelpBrowser.OpenHelpView((IHxTopic)lvi.Tag, hiliteMatches);
			}
		}

		private void ColumnClick(object sender, ColumnClickEventArgs e)
		{
			listView.ListViewItemSorter = new ListViewItemComparer(e.Column);
			listView.Sort();
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
			string text = StringParser.Parse("${res:AddIns.HtmlHelp2.ResultsOfSearchResults}",
			                                 new string[,]
			                                 {{"0", indexTerm},
			                                 	{"1", listView.Items.Count.ToString(CultureInfo.InvariantCulture)},
			                                 	{"2", (listView.Items.Count == 1)?"${res:AddIns.HtmlHelp2.SingleTopic}":"${res:AddIns.HtmlHelp2.MultiTopic}"}}
			                                );

			StatusBarService.SetMessage(text);
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
				ListViewItem itemA = x as ListViewItem;
				ListViewItem itemB = y as ListViewItem;

				switch (col)
				{
					case 2:
						int a = Int32.Parse(itemA.SubItems[col].Text, CultureInfo.InvariantCulture);
						int b = Int32.Parse(itemB.SubItems[col].Text, CultureInfo.InvariantCulture);
						if(a > b) return 1;
						else if(a < b) return -1;
						else return 0;
					default:
						return string.Compare(itemA.SubItems[col].Text, itemB.SubItems[col].Text);
				}
			}
		}
		#endregion
	}
}
