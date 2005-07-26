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
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public enum SearchResultPanelViewMode {
		Flat, 
		PerFile,
		Structural
	}
	
	public class SearchResultPanel : AbstractPadContent
	{
		static SearchResultPanel instance;
		
		public static SearchResultPanel Instance {
			get {
				return instance;
			}
		}
		
		Panel                     myPanel        = new Panel();
		ExtTreeView               resultTreeView = new ExtTreeView();
		
		string             curPattern = null;
		List<SearchResult> curResults = null;
		
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public SearchResultPanelViewMode ViewMode {
			get {
				return PropertyService.Get("SearchAndReplace.SearchResultPanelViewMode", SearchResultPanelViewMode.Flat);
			}
			set {
				PropertyService.Set("SearchAndReplace.SearchResultPanelViewMode", value);
				ShowSearchResults(curPattern, curResults);
			}
		}
		
		public void ExpandAll()
		{
			Stack<TreeNode> nodes = new Stack<TreeNode>();
			foreach (TreeNode node in resultTreeView.Nodes) {
				nodes.Push(node);
			}
			while (nodes.Count > 0) {
				TreeNode node = nodes.Pop();
				node.Expand();
				foreach (TreeNode childNode in node.Nodes) {
					nodes.Push(childNode);
				}
			}
		}
		public void Clear()
		{
			resultTreeView.Nodes.Clear();
		}
		
		public void CollapseAll()
		{
			Stack<TreeNode> nodes = new Stack<TreeNode>();
			foreach (TreeNode node in resultTreeView.Nodes) {
				nodes.Push(node);
			}
			while (nodes.Count > 0) {
				TreeNode node = nodes.Pop();
				node.Collapse();
				foreach (TreeNode childNode in node.Nodes) {
					nodes.Push(childNode);
				}
			}
		}
		
		void ShowSearchResultsPerFile()
		{
			Dictionary<string, SearchFolderNode> folderNodes = new Dictionary<string, SearchFolderNode>();
			foreach (SearchResult result in curResults) {
				if (!folderNodes.ContainsKey(result.FileName)) {
					folderNodes[result.FileName] = new SearchFolderNode(result.FileName);
				}
				folderNodes[result.FileName].Results.Add(result);
			}
			
			SearchRootNode searchRootNode = new SearchRootNode(curPattern, curResults, folderNodes.Count);
			foreach (SearchFolderNode folderNode in folderNodes.Values) {
				folderNode.SetText();
				searchRootNode.Nodes.Add(folderNode);
			}
			
			resultTreeView.Nodes.Add(searchRootNode);
			searchRootNode.Expand();
		}
		
		void ShowSearchResultsFlat()
		{
			Dictionary<string, SearchFolderNode> folderNodes = new Dictionary<string, SearchFolderNode>();
			foreach (SearchResult result in curResults) {
				if (!folderNodes.ContainsKey(result.FileName)) {
					folderNodes[result.FileName] = new SearchFolderNode(result.FileName);
				}
				folderNodes[result.FileName].Results.Add(result);
			}
			
			SearchRootNode searchRootNode = new SearchRootNode(curPattern, curResults, folderNodes.Count);
			foreach (SearchFolderNode folderNode in folderNodes.Values) {
				folderNode.PerformInitialization();
				foreach (SearchResultNode node in folderNode.Nodes) {
					node.ShowFileName = true;
					searchRootNode.Nodes.Add(node);
				}
			}
			
			resultTreeView.Nodes.Add(searchRootNode);
			searchRootNode.Expand();
		}
		
		public void ShowSearchResults(string pattern, List<SearchResult> results)
		{
			this.curPattern = pattern;
			this.curResults = results;
			if (results == null) {
				return;
			}
			resultTreeView.BeginUpdate();
			resultTreeView.Nodes.Clear();
			
			switch (ViewMode) {
				case SearchResultPanelViewMode.PerFile:
					ShowSearchResultsPerFile();
					break;
				case SearchResultPanelViewMode.Flat:
					ShowSearchResultsFlat();
					break;
			}
			
			
			resultTreeView.EndUpdate();
		}
		
		public SearchResultPanel()
		{
			instance = this;
			
			resultTreeView.Dock = DockStyle.Fill;
			resultTreeView.Font = ExtTreeNode.Font;
			resultTreeView.IsSorted = false;
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/SearchResultPanel/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { resultTreeView, toolStrip} );
		}
		
	}
}
