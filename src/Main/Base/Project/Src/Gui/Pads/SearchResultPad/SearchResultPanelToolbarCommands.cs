// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public class ExpandAll : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchResultPanel.Instance.ExpandAll();
		}
	}
	
	public class CollapseAll : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchResultPanel.Instance.CollapseAll();
		}
	}
	
	public class ShowLastSearchResults : AbstractMenuCommand
	{
		ToolBarDropDownButton dropDownButton;
		
		public override bool IsEnabled {
			get {
				return SearchResultPanel.Instance.LastSearches.Count > 0;
			}
		}
		
		public override void Run()
		{
		}
		
		void SwitchSearchResults(object sender, EventArgs e)
		{
			SearchResult result = (SearchResult)((ToolStripItem)sender).Tag;

			// "bubble" this saved search to the top of the list
			// remove search, ShowSearchResults will add it again
			SearchResultPanel.Instance.LastSearches.Remove(result);
			
			SearchResultPanel.Instance.ShowSearchResults(result);
		}
		
		void ClearHistory(object sender, EventArgs e)
		{
			SearchResultPanel.Instance.Clear();
			SearchResultPanel.Instance.LastSearches.Clear();
			UpdateLastSearches(null, null);
		}
		
		void UpdateLastSearches(object sender, EventArgs e)
		{
			dropDownButton.DropDownItems.Clear();
			foreach (SearchResult args in SearchResultPanel.Instance.LastSearches) {
				ToolStripItem newItem = new ToolStripMenuItem();
				newItem.Text = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesOf}",
				                                 new string[,] {{ "Pattern", args.Pattern }});
				if (args.Results != null) {
					newItem.Text += " (" + SearchRootNode.GetOccurencesString(args.Results.Count) + ")";
				}
				newItem.Tag  = args;
				newItem.Click += new EventHandler(SwitchSearchResults);
				dropDownButton.DropDownItems.Add(newItem);
			}
			dropDownButton.DropDownItems.Add(new ToolStripSeparator());
			ToolStripItem clearHistoryItem = new ToolStripMenuItem();
			clearHistoryItem.Text = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.ClearHistory}");
			clearHistoryItem.Click += new EventHandler(ClearHistory);
			dropDownButton.DropDownItems.Add(clearHistoryItem);
			dropDownButton.Enabled = IsEnabled;
		}
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			
			SearchResultPanel.Instance.SearchResultsShown += UpdateLastSearches;
			UpdateLastSearches(null, null);
		}
	}
	
	public class SelectViewMode : AbstractMenuCommand
	{
		ToolBarDropDownButton dropDownButton;
		
		public override void Run()
		{
		}
		
		void SetViewMode(object sender, EventArgs e)
		{
			PadDescriptor searchResultPanel = WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultPanel));
			if (searchResultPanel != null) {
				searchResultPanel.BringPadToFront();
				SearchResultPanel.Instance.ViewMode = (SearchResultPanelViewMode)((ToolStripItem)sender).Tag;	;
				UpdateDropDownItems();
			} else {
				MessageService.ShowError("SearchResultPanel can't be found.");
			}
		}

		void UpdateDropDownItems()
		{
			// Synchronize the Checked state of the menu items with
			// the current ViewMode of the SearchResultPanel.
			foreach(ToolStripItem item in dropDownButton.DropDownItems) {
				((ToolStripMenuItem)item).Checked =
					(SearchResultPanelViewMode)item.Tag == SearchResultPanel.Instance.ViewMode;
			}
		}

		void GenerateDropDownItems()
		{
			ToolStripMenuItem newItem = null;
			string menuItemText = String.Empty;
			
			// Use SearchResultPanelViewMode enum to generate the menu choices automatically.
			foreach (SearchResultPanelViewMode viewMode in System.Enum.GetValues(typeof(SearchResultPanelViewMode))) {
				newItem = new ToolStripMenuItem();
				newItem.Text = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel."+viewMode.ToString()+"}");
				newItem.Tag = viewMode;
				newItem.Click += new EventHandler(SetViewMode);
				newItem.Checked = SearchResultPanel.Instance.ViewMode == viewMode;
				dropDownButton.DropDownItems.Add(newItem);
			}
		}
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			GenerateDropDownItems();
		}
	}
	
	
}
