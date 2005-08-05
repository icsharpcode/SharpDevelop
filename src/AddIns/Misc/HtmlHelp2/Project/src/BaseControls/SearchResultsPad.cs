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
	using HtmlHelp2Service;


	public class ShowSearchResultsMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor searchResults = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchResultsPad));
			if(searchResults != null) searchResults.BringPadToFront();
		}
	}


	public class HtmlHelp2SearchResultsPad : AbstractPadContent
	{
		ListView listView         = new ListView();
		ColumnHeader title        = new ColumnHeader();
		ColumnHeader location     = new ColumnHeader();
		ColumnHeader rank         = new ColumnHeader();

		public override Control Control
		{
			get {
				return listView;
			}
		}

		public override void RedrawContent()
		{
			this.SetListViewHeader();
		}

		public ListView SearchResultsListView
		{
			get {
				return listView;
			}
		}

		public HtmlHelp2SearchResultsPad()
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
			listView.CreateControl();
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
			if(lvi != null && lvi.Tag != null && lvi.Tag is IHxTopic) {
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
			foreach(ListViewItem lvi in listView.Items) {
				if(lvi.Tag != null) { lvi.Tag = null; }
			}

			listView.Items.Clear();
		}

		public void SetStatusMessage(string indexTerm)
		{
			/*
			 * @SharpDevelop developers: I would like to have the possibility to
			 * change the Pad's title. It works without, but it would look
			 * better if I could write what was searched and how many topics are
			 * matching.
			 */
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

			public ListViewItemComparer(int column) {
				col = column;
			}

			public int Compare(object x, object y) {
				switch(col) {
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
