/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 15.12.2004
 * Time: 09:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
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
			
			toolStrip = new ToolStrip();
			toolStrip.ShowItemToolTips  = true;
			toolStrip.Dock = DockStyle.Top;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip.Stretch   = true;
			standardItems = ToolbarService.CreateToolStripItems(this, "/SharpDevelop/Pads/ProjectBrowser/ToolBar/Standard");
			if (standardItems != null) {
				toolStrip.Items.AddRange(standardItems);
			}
			Controls.Add(toolStrip);
			projectBrowserControl.TreeView.AfterSelect += new TreeViewEventHandler(TreeViewAfterSelect);
		}
		
		void TreeViewAfterSelect(object sender, TreeViewEventArgs e) 
		{
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			if (node == null) {
				toolStrip.Items.Clear();
				return;
			}
			toolStrip.Items.Clear();
			toolStrip.Items.AddRange(standardItems);
			if (node.ToolbarAddinTreePath != null) {
				toolStrip.Items.Add(new ToolStripSeparator());
				toolStrip.Items.AddRange(ToolbarService.CreateToolStripItems(node, node.ToolbarAddinTreePath));
			}
		}
		
		public void ViewSolution(Solution solution)
		{
			projectBrowserControl.ViewSolution(solution);
		}
		
		public void Clear()
		{
			projectBrowserControl.Clear();
		}
		
		public void SelectFile(string fileName)
		{
			projectBrowserControl.SelectFile(fileName);
		}
	}
}
