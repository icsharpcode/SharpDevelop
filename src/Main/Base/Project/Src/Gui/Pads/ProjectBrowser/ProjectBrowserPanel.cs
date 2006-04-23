// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectBrowserPanel.
	/// </summary>
	public class ProjectBrowserPanel : System.Windows.Forms.UserControl
	{
		ToolStrip             toolStrip;
		ProjectBrowserControl projectBrowserControl;
		ToolStripItem[]       standardItems;
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return projectBrowserControl.SelectedNode;
			}
		}
		
		public AbstractProjectBrowserTreeNode RootNode {
			get {
				return projectBrowserControl.RootNode;
			}
		}
		
		public ProjectBrowserControl ProjectBrowserControl {
			get {
				return projectBrowserControl;
			}
		}
		
		public ProjectBrowserPanel()
		{
			projectBrowserControl      = new ProjectBrowserControl();
			projectBrowserControl.Dock = DockStyle.Fill;
			Controls.Add(projectBrowserControl);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ProjectBrowser/ToolBar/Standard");
			toolStrip.ShowItemToolTips  = true;
			toolStrip.Dock = DockStyle.Top;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip.Stretch   = true;
			standardItems = new ToolStripItem[toolStrip.Items.Count];
			toolStrip.Items.CopyTo(standardItems, 0);
			Controls.Add(toolStrip);
			projectBrowserControl.TreeView.BeforeSelect += TreeViewBeforeSelect;
		}
		
		void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			UpdateToolStrip(e.Node as AbstractProjectBrowserTreeNode);
		}
		
		void UpdateToolStrip(AbstractProjectBrowserTreeNode node)
		{
			toolStrip.Items.Clear();
			toolStrip.Items.AddRange(standardItems);
			ToolbarService.UpdateToolbar(toolStrip);
			if (node != null && node.ToolbarAddinTreePath != null) {
				toolStrip.Items.Add(new ToolStripSeparator());
				toolStrip.Items.AddRange((ToolStripItem[])AddInTree.BuildItems(node.ToolbarAddinTreePath, node, false).ToArray(typeof(ToolStripItem)));
			}
		}
		
		public void ViewSolution(Solution solution)
		{
			UpdateToolStrip(null);
			projectBrowserControl.ViewSolution(solution);
		}
		
		/// <summary>
		/// Writes the current view state into the memento.
		/// </summary>
		public void StoreViewState(Properties memento)
		{
			memento.Set("ProjectBrowserState", GetViewStateString(projectBrowserControl.TreeView));
		}
		
		/// <summary>
		/// Reads the view state from the memento.
		/// </summary>
		public void ReadViewState(Properties memento)
		{
			ApplyViewStateString(memento.Get("ProjectBrowserState", ""), projectBrowserControl.TreeView);
		}
		
		// example ViewStateString:
		// [Main[ICSharpCode.SharpDevelop[Src[Gui[Pads[ProjectBrowser[]]]]Services[]]]]
		// -> every node name is terminated by opening bracket
		// -> only expanded nodes are included in the view state string
		// -> after an opening bracket, an identifier or closing bracket must follow
		// -> after a closing bracket, an identifier or closing bracket must follow
		// -> nodes whose text contains '[' can not be saved
		public static string GetViewStateString(TreeView treeView)
		{
			StringBuilder b = new StringBuilder();
			WriteViewStateString(b, treeView.Nodes[0]);
			return b.ToString();
		}
		static void WriteViewStateString(StringBuilder b, TreeNode node)
		{
			b.Append('[');
			foreach (TreeNode subNode in node.Nodes) {
				if (subNode.IsExpanded && subNode.Text.IndexOf('[') < 0) {
					b.Append(subNode.Text);
					WriteViewStateString(b, subNode);
				}
			}
			b.Append(']');
		}
		public static void ApplyViewStateString(string viewState, TreeView treeView)
		{
			if (viewState.Length == 0)
				return;
			int i = 0;
			ApplyViewStateString(treeView.Nodes[0], viewState, ref i);
			System.Diagnostics.Debug.Assert(i == viewState.Length - 1);
		}
		static void ApplyViewStateString(TreeNode node, string viewState, ref int pos)
		{
			if (viewState[pos++] != '[')
				throw new ArgumentException("pos must point to '['");
			// expect an identifier or an closing bracket
			while (viewState[pos] != ']') {
				StringBuilder nameBuilder = new StringBuilder();
				char ch;
				while ((ch = viewState[pos++]) != '[') {
					nameBuilder.Append(ch);
				}
				pos -= 1; // go back to '[' character
				string nodeText = nameBuilder.ToString();
				// find the node in question
				TreeNode subNode = null;
				if (node != null) {
					foreach (TreeNode n in node.Nodes) {
						if (n.Text == nodeText) {
							subNode = n;
							break;
						}
					}
				}
				if (subNode != null) {
					subNode.Expand();
				}
				ApplyViewStateString(subNode, viewState, ref pos);
				// pos now points to the closing bracket of the inner view state
				pos += 1; // move to next character
			}
		}
		
		public void Clear()
		{
			projectBrowserControl.Clear();
			UpdateToolStrip(null);
		}
		
		public void SelectFile(string fileName)
		{
			projectBrowserControl.SelectFile(fileName);
		}
	}
}
