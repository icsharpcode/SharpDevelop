// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			if (node == null) {
				toolStrip.Items.Clear();
				return;
			}
			toolStrip.Items.Clear();
			toolStrip.Items.AddRange(standardItems);
			if (node.ToolbarAddinTreePath != null) {
				toolStrip.Items.Add(new ToolStripSeparator());
				toolStrip.Items.AddRange((ToolStripItem[])AddInTree.BuildItems(node.ToolbarAddinTreePath, node, false).ToArray(typeof(ToolStripItem)));
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
