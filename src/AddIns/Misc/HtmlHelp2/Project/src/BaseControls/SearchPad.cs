/* ***********************************************************
 * 
 * Help 2.0 Environment for SharpDevelop
 * Search Pad
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 * 
 * ********************************************************* */
namespace HtmlHelp2
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.SharpDevelop.Project;
	using HtmlHelp2.Environment;
	using HtmlHelp2.HelperDialog;
	using MSHelpServices;


	public class ShowSearchMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
			if (search != null)
			{
				search.BringPadToFront();
				((HtmlHelp2SearchPad)search.PadContent).FocusSearchTextBox();
			}
		}
	}

	public class HtmlHelp2SearchPad : AbstractPadContent
	{
		Panel mainPanel            = new Panel();
		Button searchButton        = new Button();
		ComboBox filterCombobox    = new ComboBox();
		ComboBox searchTerm        = new ComboBox();
		CheckBox titlesOnly        = new CheckBox();
		CheckBox enableStemming    = new CheckBox();
		CheckBox reuseMatches      = new CheckBox();
		CheckBox hiliteTopics      = new CheckBox();
		Label label1               = new Label();
		Label label2               = new Label();
		string selectedQuery       = "";
		bool searchIsBusy          = false;

		public override Control Control
		{
			get { return mainPanel; }
		}
		
		public void FocusSearchTextBox()
		{
			searchTerm.Focus();
		}

		public override void RedrawContent()
		{
			searchButton.Text       = StringParser.Parse("${res:AddIns.HtmlHelp2.Search}");
			titlesOnly.Text         = StringParser.Parse("${res:AddIns.HtmlHelp2.SearchInTitlesOnly}");
			enableStemming.Text     = StringParser.Parse("${res:AddIns.HtmlHelp2.LookForSimilarWords}");
			reuseMatches.Text       = StringParser.Parse("${res:AddIns.HtmlHelp2.SearchInPreviouslyFoundTopics}");
			hiliteTopics.Text       = StringParser.Parse("${res:AddIns.HtmlHelp2.HighlightMatches}");
			label1.Text             = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
			label2.Text             = StringParser.Parse("${res:AddIns.HtmlHelp2.LookFor}");
		}
		
		public bool HiliteEnabled
		{
			get { return hiliteTopics.Checked; }
		}

		public HtmlHelp2SearchPad()
		{
			this.InitializeComponents();
		}

		private void InitializeComponents()
		{
			// Search controls
			Panel panel3                          = new Panel();
			mainPanel.Controls.Add(panel3);
			panel3.Width                          = 500;
			int pw                                = panel3.Width;

			panel3.Controls.Add(searchButton);
			searchButton.Enabled                  = false;
			searchButton.Text                     = StringParser.Parse("${res:AddIns.HtmlHelp2.Search}");
			searchButton.Click                   += new EventHandler(SearchButtonClick);
			panel3.Controls.Add(titlesOnly);
			panel3.Controls.Add(enableStemming);
			panel3.Controls.Add(reuseMatches);
			panel3.Controls.Add(hiliteTopics);

			titlesOnly.Width                      = pw;
			titlesOnly.Text                       = StringParser.Parse("${res:AddIns.HtmlHelp2.SearchInTitlesOnly}");
			titlesOnly.Top                        = searchButton.Top + searchButton.Height + 10;
			titlesOnly.TextAlign                  = ContentAlignment.MiddleLeft;
			titlesOnly.Enabled                    = HtmlHelp2Environment.IsReady;

			enableStemming.Width                  = pw;
			enableStemming.Text                   = StringParser.Parse("${res:AddIns.HtmlHelp2.LookForSimilarWords}");
			enableStemming.Top                    = titlesOnly.Top + titlesOnly.Height - 4;
			enableStemming.TextAlign              = ContentAlignment.MiddleLeft;
			enableStemming.Enabled                = HtmlHelp2Environment.IsReady;

			reuseMatches.Width                    = pw;
			reuseMatches.Top                      = enableStemming.Top + enableStemming.Height - 4;
			reuseMatches.Text                     = StringParser.Parse("${res:AddIns.HtmlHelp2.SearchInPreviouslyFoundTopics}");
			reuseMatches.Enabled                  = false;
			reuseMatches.TextAlign                = ContentAlignment.MiddleLeft;

			hiliteTopics.Width                    = pw;
			hiliteTopics.Top                      = reuseMatches.Top + reuseMatches.Height - 4;
			hiliteTopics.Text                     = StringParser.Parse("${res:AddIns.HtmlHelp2.HighlightMatches}");
			hiliteTopics.TextAlign                = ContentAlignment.MiddleLeft;
			hiliteTopics.Enabled                  = HtmlHelp2Environment.IsReady;
			hiliteTopics.Checked                  = true;

			panel3.Dock                           = DockStyle.Fill;

			// Filter Combobox
			Panel panel1                          = new Panel();
			mainPanel.Controls.Add(panel1);
			panel1.Dock                           = DockStyle.Top;
			panel1.Height                         = filterCombobox.Height + 15;
			panel1.Controls.Add(filterCombobox);
			filterCombobox.Dock                   = DockStyle.Top;
			filterCombobox.DropDownStyle          = ComboBoxStyle.DropDownList;
			filterCombobox.Sorted                 = true;
			filterCombobox.Enabled                = HtmlHelp2Environment.IsReady;
			filterCombobox.SelectedIndexChanged  += new EventHandler(FilterChanged);
			
			if(HtmlHelp2Environment.IsReady)
			{
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				HtmlHelp2Environment.FilterQueryChanged += new EventHandler(FilterQueryChanged);
				HtmlHelp2Environment.NamespaceReloaded  += new EventHandler(NamespaceReloaded);
			}

			// Filter label
			mainPanel.Controls.Add(label1);
			label1.Text                           = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
			label1.Dock                           = DockStyle.Top;
			label1.TextAlign                      = ContentAlignment.MiddleLeft;
			label1.Enabled                        = HtmlHelp2Environment.IsReady;

			// SearchTerm Combobox
			Panel panel2                          = new Panel();
			mainPanel.Controls.Add(panel2);
			panel2.Dock                           = DockStyle.Top;
			panel2.Height                         = searchTerm.Height + 7;
			panel2.Controls.Add(searchTerm);
			searchTerm.Dock                       = DockStyle.Top;
			searchTerm.TextChanged               += new EventHandler(SearchTextChanged);
			searchTerm.KeyPress                  += new KeyPressEventHandler(KeyPressed);
			searchTerm.Enabled                    = HtmlHelp2Environment.IsReady;

			mainPanel.Controls.Add(label2);
			label2.Text                           = StringParser.Parse("${res:AddIns.HtmlHelp2.LookFor}");
			label2.Dock                           = DockStyle.Top;
			label2.TextAlign                      = ContentAlignment.MiddleLeft;
			label2.Enabled                        = HtmlHelp2Environment.IsReady;
		}

		private void FilterChanged(object sender, EventArgs e)
		{
			object selectedItem = filterCombobox.SelectedItem;
			if(selectedItem != null)
			{
				selectedQuery = HtmlHelp2Environment.FindFilterQuery(selectedItem.ToString());
			}
		}

		#region Help 2.0 Environment Events
		private void FilterQueryChanged(object sender, EventArgs e)
		{
			mainPanel.Refresh();

			string currentFilterName = filterCombobox.SelectedItem.ToString();
			if(String.Compare(currentFilterName, HtmlHelp2Environment.CurrentFilterName) != 0)
			{
				filterCombobox.SelectedIndexChanged -= new EventHandler(FilterChanged);
				filterCombobox.SelectedIndex         = filterCombobox.Items.IndexOf(HtmlHelp2Environment.CurrentFilterName);
				selectedQuery                        = HtmlHelp2Environment.CurrentFilterQuery;
				filterCombobox.SelectedIndexChanged += new EventHandler(FilterChanged);
			}
		}

		private void NamespaceReloaded(object sender, EventArgs e)
		{
			searchTerm.Text                      = "";
			searchTerm.Items.Clear();
			filterCombobox.SelectedIndexChanged -= new EventHandler(FilterChanged);
			HtmlHelp2Environment.BuildFilterList(filterCombobox);
			filterCombobox.SelectedIndexChanged += new EventHandler(FilterChanged);
		}
		#endregion

		private void SearchButtonClick(object sender, EventArgs e)
		{
			if(searchTerm.Text != "")
			{
				this.AddTermToList(searchTerm.Text);
				this.PerformFTS(searchTerm.Text);
			}
		}

		private void SearchTextChanged(object sender, EventArgs e)
		{
			searchButton.Enabled = (searchTerm.Text != "");
		}

		private void KeyPressed(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13 && searchTerm.Text != null)
			{
				e.Handled = true;
				this.AddTermToList(searchTerm.Text);
				this.PerformFTS(searchTerm.Text);
			}
		}

		private void AddTermToList(string searchText)
		{
			if(searchTerm.Items.IndexOf(searchText) == -1)
			{
				searchTerm.Items.Insert(0, searchText);
				if(searchTerm.Items.Count > 10) searchTerm.Items.RemoveAt(10);
				searchTerm.SelectedIndex = 0;
			}
		}

		#region FTS
		private void PerformFTS(string searchWord)
		{
			this.PerformFTS(searchWord, false);
		}

		private void PerformFTS(string searchWord, bool useDynamicHelp)
		{
			if(!HtmlHelp2Environment.IsReady || searchIsBusy)
				return;

			HtmlHelp2SearchResultsView searchResults = HtmlHelp2SearchResultsView.Instance;

			try
			{
				searchIsBusy                 = true;
				IHxTopicList matchingTopics  = null;

				HxQuery_Options searchFlags  = HxQuery_Options.HxQuery_No_Option;
				searchFlags                 |= (titlesOnly.Checked)?HxQuery_Options.HxQuery_FullTextSearch_Title_Only:HxQuery_Options.HxQuery_No_Option;
				searchFlags                 |= (enableStemming.Checked)?HxQuery_Options.HxQuery_FullTextSearch_Enable_Stemming:HxQuery_Options.HxQuery_No_Option;
				searchFlags                 |= (reuseMatches.Checked)?HxQuery_Options.HxQuery_FullTextSearch_SearchPrevious:HxQuery_Options.HxQuery_No_Option;

				HtmlHelp2Dialog searchDialog = new HtmlHelp2Dialog();
				searchDialog.Text            = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSearchCaption}");
				searchDialog.ActionLabel     = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSearchInProgress}",
				                                                  new string[,]
				                                                  {{"0", searchWord}});
				searchDialog.Show();
				Application.DoEvents();
				Cursor.Current     = Cursors.WaitCursor;
				if(useDynamicHelp)
					matchingTopics = HtmlHelp2Environment.GetMatchingTopicsForDynamicHelp(searchWord);
				else
					matchingTopics = HtmlHelp2Environment.FTS.Query(searchWord, searchFlags);

				Cursor.Current     = Cursors.Default;
				searchDialog.Dispose();

				try
				{
					searchResults.CleanUp();
					searchResults.SearchResultsListView.BeginUpdate();

					foreach(IHxTopic topic in matchingTopics)
					{
						ListViewItem lvi = new ListViewItem();
						lvi.Text         = topic.get_Title(HxTopicGetTitleType.HxTopicGetRLTitle,
						                                   HxTopicGetTitleDefVal.HxTopicGetTitleFileName);
						lvi.Tag          = topic;
						lvi.SubItems.Add(topic.Location);
						lvi.SubItems.Add(topic.Rank.ToString());

						searchResults.SearchResultsListView.Items.Add(lvi);
					}

					reuseMatches.Enabled = true;
				}
				finally
				{
					searchResults.SearchResultsListView.EndUpdate();
					searchResults.SetStatusMessage(searchTerm.Text);
					searchResults.BringPadToFront();
					searchIsBusy = false;
				}
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: cannot get matching search word; " + ex.ToString());
			}
		}

		public bool PerformF1FTS(string keyword)
		{
			return this.PerformF1FTS(keyword, false);
		}

		public bool PerformF1FTS(string keyword, bool useDynamicHelp)
		{
			if(!HtmlHelp2Environment.IsReady || searchIsBusy)
				return false;

			this.PerformFTS(keyword, useDynamicHelp);

			HtmlHelp2SearchResultsView searchResults = HtmlHelp2SearchResultsView.Instance;
			return searchResults.SearchResultsListView.Items.Count > 0;
		}
		#endregion
	}
}
