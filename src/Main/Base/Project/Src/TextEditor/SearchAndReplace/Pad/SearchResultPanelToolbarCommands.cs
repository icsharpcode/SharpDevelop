// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop;

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
				return SearchReplaceInFilesManager.LastSearches.Count > 0;
			}
		}
		
		public override void Run()
		{
		}
		
		void SwitchSearchResults(object sender, EventArgs e)
		{
			SearchAllFinishedEventArgs args = (SearchAllFinishedEventArgs)((ToolStripItem)sender).Tag;
			PadDescriptor searchResultPanel = WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultPanel));
			if (searchResultPanel != null) {
				searchResultPanel.BringPadToFront();
				SearchResultPanel.Instance.ShowSearchResults(args.Pattern, args.Results);
			} else {
				MessageService.ShowError("SearchResultPanel can't be found.");
			}
		}
		
		void ClearHistory(object sender, EventArgs e)
		{
			SearchResultPanel.Instance.Clear();
			SearchReplaceInFilesManager.LastSearches.Clear();
			UpdateLastSearches(null, null);
		}
		
		void UpdateLastSearches(object sender, SearchAllFinishedEventArgs e)
		{
			dropDownButton.DropDownItems.Clear();
			foreach (SearchAllFinishedEventArgs args in SearchReplaceInFilesManager.LastSearches) {
				ToolStripItem newItem = new ToolStripMenuItem();
				newItem.Text = "Occurrences of '" +  args.Pattern + "' (" + args.Results.Count + " occurences)";
				newItem.Tag  = args;
				newItem.Click += new EventHandler(SwitchSearchResults);
				dropDownButton.DropDownItems.Add(newItem);
			}
			dropDownButton.DropDownItems.Add(new ToolStripSeparator());
			ToolStripItem clearHistoryItem = new ToolStripMenuItem();
			clearHistoryItem.Text = "<Clear History>";
			clearHistoryItem.Click += new EventHandler(ClearHistory);
			dropDownButton.DropDownItems.Add(clearHistoryItem);
			dropDownButton.Enabled = IsEnabled;
		}
		
		protected override void OnOwnerChanged(EventArgs e) 
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			
			SearchReplaceInFilesManager.SearchAllFinished += new SearchAllFinishedEventHandler(UpdateLastSearches);
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
				SearchResultPanel.Instance.ViewMode = (SearchResultPanelViewMode)((ToolStripItem)sender).Tag;
			} else {
				MessageService.ShowError("SearchResultPanel can't be found.");
			}
		}
		
		void GenerateDropDownItems()
		{
			ToolStripItem newItem = new ToolStripMenuItem();
			newItem.Text = "Per file";
			newItem.Tag  = SearchResultPanelViewMode.PerFile;
			newItem.Click += new EventHandler(SetViewMode);
			dropDownButton.DropDownItems.Add(newItem);
			
			newItem = new ToolStripMenuItem();
			newItem.Text = "Flat";
			newItem.Tag  = SearchResultPanelViewMode.Flat;
			newItem.Click += new EventHandler(SetViewMode);
			dropDownButton.DropDownItems.Add(newItem);
		}
		
		protected override void OnOwnerChanged(EventArgs e) 
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			GenerateDropDownItems();
		}
	}
	
	
}
