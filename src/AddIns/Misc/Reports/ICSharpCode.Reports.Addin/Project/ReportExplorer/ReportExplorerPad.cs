// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	internal sealed  class ReportExplorerPad : AbstractPadContent,INotifyPropertyChanged
	{
		private static int viewCount;
		private ExplorerTree explorerTree;
		private static ReportExplorerPad instance;
		private ReportModel reportModel;
		/// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>
		
		
		public ReportExplorerPad():base()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			WorkbenchSingleton.Workbench.ViewClosed += ActiveViewClosed;
			this.explorerTree = new ExplorerTree();
			this.explorerTree.MouseDown += new MouseEventHandler(ReportExplorer_MouseDown);
			this.explorerTree.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportExplorerPad_PropertyChanged);
			instance = this;
		}

		
		void ReportExplorerPad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.NotifyReportView(e.PropertyName);
		}
		
		#region Setup
		
		
		
		public void AddContent (ReportModel reportModel)
		{
			
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.reportModel = reportModel;
			this.explorerTree.ReportModel = this.reportModel;
			ViewCount++;
		}
	
		#endregion
		
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			ReportDesignerView vv = WorkbenchSingleton.Workbench.ActiveViewContent as ReportDesignerView;
			if (vv != null) {
				Console.WriteLine("Explorerpad:ActiveViewContentChanged {0}",vv.TitleName);
			}
		}
		
		void ActiveViewClosed (object source, ViewContentEventArgs e)
		{
			if (e.Content is ReportDesignerView) {
				Console.WriteLine ("Designer closed");
				                   ViewCount --;
			}
		}
		
		#region Mouse
		
		private void ReportExplorer_MouseDown (object sender, MouseEventArgs e)
		{
			AbstractFieldsNode abstrNode =  this.explorerTree.GetNodeAt(e.X, e.Y) as AbstractFieldsNode;
			if (e.Button == MouseButtons.Right) {
				this.explorerTree.SelectedNode = abstrNode;
				if (abstrNode != null) {
					if (abstrNode.ContextMenuAddinTreePath.Length > 0) {
						ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,abstrNode.ContextMenuAddinTreePath);
						ctMen.Show (this.Control,new Point (e.X,e.Y));
					}
				}
			}
		}
		
		#endregion
		
		
		#region DragDrop
		/*
		void TreeViewItemDrag (object sender,ItemDragEventArgs e) 
		{
			
			ColumnNode node = (ColumnNode)e.Item;
			if (node != null) {
				if (node.ImageIndex == ExplorerTree.ColumnIcon) {
					this.treeView.SelectedNode = node;
					if (node != null) {
						this.treeView.DoDragDrop(node.DragDropDataObject,
						                         DragDropEffects.Copy | DragDropEffects.Scroll);
					}
				}
			}
			
		}
		
		*/
		
		
		void TreeViewDragDrop (object sender,DragEventArgs e)
		{
			/*
			if(e.Data.GetDataPresent(ReportExplorer.DragNodeDescription, false)){
				Point pt = this.treeView.PointToClient (new Point( e.X,e.Y));

				SectionTreeNode sectionNode = this.treeView.GetNodeAt (pt) as SectionTreeNode;
				
				if (sectionNode != null) {
					// If we dragdrop to GroupNode, remove all subnods in SortNode
					if (this.treeView.IsGroupNode(sectionNode)) {
						this.treeView.ClearSortNode();
					} 
					ColumnsTreeNode t = (ColumnsTreeNode)e.Data.GetData(ReportExplorer.DragNodeDescription, true);
					ColumnsTreeNode newNode = new ColumnsTreeNode (t.FieldName);

					// Useless to add a node twice
					if (!ExplorerTree.NodeExist (sectionNode,newNode)) {
						newNode.SortDirection = ListSortDirection.Ascending;
						newNode.ImageIndex = ExplorerTree.AscendingIcon;
						newNode.SelectedImageIndex = ExplorerTree.AscendingIcon;
						this.treeView.SelectedNode = (TreeNode)newNode;
						this.treeView.CheckNode (newNode);
						sectionNode.Nodes.Add(newNode);
						ReportExplorer.NotifyReportView();
					}
				}
			}
			*/
		}
		

		#endregion
		
		
		#region publics for Commands
	
		// These public methods are all called from ExplorerCommands
		
		public void ClearNodes () 
		{
			this.explorerTree.ClearSection();
		}
		
		
		public void ToggleOrder ()
		{
			this.explorerTree.ToggleSortOrder();
		}
		
		
		public void RemoveNode()
		{
			this.explorerTree.RemoveNode();
		}
		
		public void RefreshParameters()
		{
			this.explorerTree.BuildTree();
			this.NotifyReportView("Parameters");
		}
		
		#endregion
		
		public static ReportExplorerPad Instance {
			get { return instance; }
		}
		
		
		public static int ViewCount {
			get { return viewCount; }
			set {
				viewCount = value;
				if (viewCount == 0)	{
					Console.WriteLine("Should find a way to close/hide a pad");
				}
			}
		}
		
		
		
		public ReportModel ReportModel 
		{get {return this.reportModel;}}
		
		public	ColumnCollection SortColumnCollection
		{
			get {return this.explorerTree.SortColumnCollection;}
		}
		
		#region IPropertyChanged
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private  void NotifyReportView(string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this,new System.ComponentModel.PropertyChangedEventArgs(property));                     
			}
		}
		
		#endregion
		
		#region AbstractPadContent
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control 
		{
			get {
				return this.explorerTree;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
			this.explorerTree.Dispose();
		}
		
		#endregion
	}
}
