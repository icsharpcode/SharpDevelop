/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Search results Pad
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2
{
	using System;
	using System.Collections;
	using System.Windows.Forms;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpServices;
	using HtmlHelp2.Environment;


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
		
		public void BringPadToFront()
		{
			PadDescriptor descriptor = WorkbenchSingleton.Workbench.GetPad(typeof(SearchAndReplace.SearchResultPanel));
			SearchAndReplace.SearchResultPanel srp = (SearchAndReplace.SearchResultPanel)descriptor.PadContent;
			srp.ShowSpecialPanel(this);
			descriptor.BringPadToFront();
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
			listView.Enabled           = HtmlHelp2Environment.IsReady;
			listView.Alignment         = ListViewAlignment.Left;
			listView.View              = View.Details;
			listView.Dock              = DockStyle.Fill;
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
			                                 	{"1", listView.Items.Count.ToString()},
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
				switch (col)
				{
					case 2:
						int a = Int32.Parse(((ListViewItem)x).SubItems[col].Text);
						int b = Int32.Parse(((ListViewItem)y).SubItems[col].Text);
						if(a > b) return 1;
						else if(a < b) return -1;
						else return 0;
					default:
						return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
				}
			}
		}
		#endregion
	}
}
