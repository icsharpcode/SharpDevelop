// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
	public class SearchResultPanel : AbstractPadContent
	{
		static SearchResultPanel instance;
		
		public static SearchResultPanel Instance {
			get {
				return instance;
			}
		}
		
		Panel       myPanel        = new Panel();
		ExtTreeView resultTreeView = new ExtTreeView();
		
		public override Control Control {
			get {
				return myPanel;
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
		
		public void ShowSearchResults(string pattern, List<SearchResult> results)
		{
			Dictionary<string, SearchFolderNode> folderNodes = new Dictionary<string, SearchFolderNode>();
			foreach (SearchResult result in results) {
				TreeNode newResult = new TreeNode();
				if (!folderNodes.ContainsKey(result.FileName)) {
					folderNodes[result.FileName] = new SearchFolderNode(result.FileName);
				}
				folderNodes[result.FileName].Results.Add(result);
			}
			
			SearchRootNode searchRootNode = new SearchRootNode(pattern, results);
			foreach (SearchFolderNode folderNode in folderNodes.Values) {
				folderNode.SetText();
				searchRootNode.Nodes.Add(folderNode);
			}
			resultTreeView.Nodes.Clear();
			resultTreeView.Nodes.Add(searchRootNode);
			searchRootNode.Expand();
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
