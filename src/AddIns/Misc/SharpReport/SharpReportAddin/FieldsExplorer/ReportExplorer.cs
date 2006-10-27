/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 01.08.2006
 * Time: 13:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using SharpReportCore;

namespace SharpReportAddin
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	public class ReportExplorer : AbstractPadContent{
		
		Panel contentPanel = new Panel();
		ExplorerTree treeView = new ExplorerTree();
		
		/// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>
		
		public ReportExplorer():base(){
			this.contentPanel.Controls.Add(this.treeView);
			this.treeView.ItemDrag += TreeViewItemDrag;
			this.treeView.DragDrop += TreeViewDragDrop;
			this.treeView.DragOver += TreeViewDragOver;
		}
		
		#region DragDrop
		
		void TreeViewItemDrag (object sender,ItemDragEventArgs e) {
			ColumnsTreeNode node = (ColumnsTreeNode)e.Item;
			if (node != null) {
				if (node.ImageIndex == this.treeView.ColumnIcon) {
					this.treeView.SelectedNode = node;
					if (node != null) {
						this.treeView.DoDragDrop(node.DragDropDataObject,
						                         DragDropEffects.Copy | DragDropEffects.Scroll);
					}
				}
			}
		}
		
		
		void TreeViewDragOver (object sender,DragEventArgs e) {
			
			TreeNode node  = this.treeView.GetNodeAt(this.treeView.PointToClient(new Point (e.X,e.Y)));
			
			node.EnsureVisible();
			if (node.Nodes.Count > 0) {
				node.Expand();
			}
			if(e.Data.GetDataPresent("SharpReportAddin.ColumnsTreeNode", false)){
				//If we are in the AvailableFields Section we can't drop
				if (node is SectionTreeNode){
					e.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
				} else {
					e.Effect = DragDropEffects.None;
				}
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		
		void TreeViewDragDrop (object sender,DragEventArgs e) {
			
			if(e.Data.GetDataPresent("SharpReportAddin.ColumnsTreeNode", false)){
				Point pt = this.treeView.PointToClient (new Point( e.X,e.Y));

				SectionTreeNode node = this.treeView.GetNodeAt (pt) as SectionTreeNode;
				
				if (node != null) {
					// If we dragdrop to GroupNode, remove all subnods in SortNode
					if (this.treeView.IsGroupNode(node)) {
						this.treeView.ClearSortNode();
					} 
					
					ColumnsTreeNode t = (ColumnsTreeNode)e.Data.GetData("SharpReportAddin.ColumnsTreeNode", true);
					ColumnsTreeNode dest = new ColumnsTreeNode (t.Text);

					// Useless to add a node twice
					if (!ExplorerTree.CheckForExist (node,dest)) {
						dest.SortDirection = ListSortDirection.Ascending;
						dest.ImageIndex = this.treeView.AscendingIcon;
						dest.SelectedImageIndex = this.treeView.AscendingIcon;
						this.treeView.SelectedNode = (TreeNode)dest;
						this.treeView.CheckNode (dest);
						node.Nodes.Add(dest);
						NotifyReportView();
					}
				}
			}
		}
		

		
		#endregion
		
		#region publics for Commands
	
		// These public methods are all called from ExplorerCommands
		public void ClearAndRebuildTree() {
			this.treeView.ClearAndFill();
		}
		
		public void ClearNodes () {
			if (this.treeView.SelectedNode is SectionTreeNode) {
				if (this.treeView.SelectedNode.Nodes.Count > 0) {
					this.treeView.SelectedNode.Nodes.Clear();
					NotifyReportView();
				}
			}
		}
		
		
		public void ToggleOrder () {
			this.treeView.ToggleOrder();
			this.NotifyReportView();
		}
		
		public void RemoveNode() {
			if (this.treeView.SelectedNode != null) {
				TreeNode parent = this.treeView.SelectedNode.Parent;
				this.treeView.SelectedNode.Remove();
				this.treeView.SelectedNode = parent;
				NotifyReportView();
			}
		}
		
		
		private void NotifyReportView() {
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent is SharpReportView) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsDirty = true;
			}
		}
		
		#endregion
		
		
		
		public void Update (ReportModel model){
			this.treeView.CollectModel(model);
		}
		
		
		#region properties
		
		public ReportModel ReportModel {
			set {
				this.treeView.FillTree(value);
			}
		}
		#endregion
		
		
		
		#region AbstractPadContent
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control {
			get {
				return this.contentPanel;
			}
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent(){
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose(){
			this.contentPanel.Dispose();
		}
		#endregion
		
	}
}
