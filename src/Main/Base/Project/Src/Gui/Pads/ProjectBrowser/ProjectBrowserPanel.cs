// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
			
			if (SD.AddInTree.GetTreeNode("/SharpDevelop/Pads/ProjectBrowser/ToolBar/Standard", false) != null) {
				toolStrip = SD.WinForms.ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ProjectBrowser/ToolBar/Standard");
				toolStrip.ShowItemToolTips  = true;
				toolStrip.Dock = DockStyle.Top;
				toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
				toolStrip.Stretch   = true;
				standardItems = new ToolStripItem[toolStrip.Items.Count];
				toolStrip.Items.CopyTo(standardItems, 0);
				Controls.Add(toolStrip);
			}
			projectBrowserControl.TreeView.BeforeSelect += TreeViewBeforeSelect;
		}
		
		void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			UpdateToolStrip(e.Node as AbstractProjectBrowserTreeNode);
		}
		
		void UpdateToolStrip(AbstractProjectBrowserTreeNode node)
		{
			if (toolStrip == null) return;
			toolStrip.Items.Clear();
			toolStrip.Items.AddRange(standardItems);
			SD.WinForms.ToolbarService.UpdateToolbar(toolStrip);
			if (node != null && node.ToolbarAddinTreePath != null) {
				toolStrip.Items.Add(new ToolStripSeparator());
				toolStrip.Items.AddRange(SD.WinForms.ToolbarService.CreateToolStripItems(node.ToolbarAddinTreePath, node, false));
			}
		}
		
		public void ViewSolution(ISolution solution)
		{
			UpdateToolStrip(null);
			projectBrowserControl.ViewSolution(solution);
		}
		
		/// <summary>
		/// Writes the current view state into the memento.
		/// </summary>
		public void StoreViewState(Properties memento)
		{
			projectBrowserControl.StoreViewState(memento);
		}
		
		/// <summary>
		/// Reads the view state from the memento.
		/// </summary>
		public void ReadViewState(Properties memento)
		{
			projectBrowserControl.ReadViewState(memento);
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
