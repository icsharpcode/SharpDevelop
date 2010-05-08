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

namespace ICSharpCode.Reports.Addin
{
	
	/// <summary>
	/// Description of ExplorerTree.
	/// </summary>
	internal class ExplorerTree:TreeView,INotifyPropertyChanged
	{
		private const string sortColumnMenu = "/SharpDevelopReports/ContextMenu/FieldsExplorer/ColumnTreeNode";
		private const string sectionContextMenu = "/SharpDevelopReports/ContextMenu/FieldsExplorer/SectionTreeNode";
		private const string parameterEditorMenu = "/SharpDevelopReports/ContextMenu/FieldsExplorer/ParameterNode";
		private SectionNode nodeRoot;
		private SectionNode nodeModel;

		private SectionNode nodeAvailableFields;
		private SectionNode nodeSorting;
		private SectionNode nodeGrouping;
		private TreeNode nodeFunction;
		private SectionNode nodeParams;
		
		private static int folderClosed = 0;
		private static int folderOpen  = 1;
		private static int clearIcon = 2;
		
		private static int ascendingIcon = 4;
		private static int descendingIcon = 5;
//		private static int storedprocIcon = 7;

		private static int columnIcon = 8;
	//	private static int functionIcon = 9;
		
		private ReportModel reportModel;
	
		public ExplorerTree():base()
		{
			LabelEdit     = true;
			AllowDrop     = true;
			HideSelection = false;
			Dock          = DockStyle.Fill;
			Scrollable = true;
			this.InitImageList();
			this.AllowDrop = true;
		}
		
		
		#region DragDrop
	
		[EditorBrowsableAttribute()]
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			TreeNode node = GetNodeAt(e.X, e.Y);
			if (node != null) {
				if (SelectedNode != node) {
					SelectedNode = node;
					this.DoDragDrop(SelectedNode, DragDropEffects.Copy);
				}
			} 
		}
		
		
		[EditorBrowsableAttribute()]
		protected override void OnDragOver(DragEventArgs drgevent)
		{
			base.OnDragOver(drgevent);
			TreeNode node  = this.GetNodeAt(this.PointToClient(new Point (drgevent.X,drgevent.Y)));
			
			node.EnsureVisible();
			if (node.Nodes.Count > 0) {
				node.Expand();
			}
			
			if (drgevent.Data.GetData(typeof(ColumnNode)) != null)
			{
				if (node is SectionNode){
					drgevent.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
				} else {
					drgevent.Effect = DragDropEffects.None;
				}
			}
		}
	
		
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			ColumnNode node = e.Item as ColumnNode;
			if (node != null) {
				if (node.ImageIndex == ExplorerTree.columnIcon) {
					this.SelectedNode = node;
					if (node != null) {
						this.DoDragDrop(node,DragDropEffects.Copy | DragDropEffects.Scroll);       
					}
				}
			}
		}
		
		
		[EditorBrowsableAttribute()]
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			base.OnDragDrop(drgevent);
			ColumnNode node = (ColumnNode)drgevent.Data.GetData(typeof(ColumnNode));
			Point pt = this.PointToClient (new Point( drgevent.X,drgevent.Y));
			SectionNode sectionNode = this.GetNodeAt (pt) as SectionNode;
			if (sectionNode != null) {
				if (sectionNode == this.nodeGrouping) {
					this.nodeSorting.Nodes.Clear();
				}
				
				if (!ExplorerTree.NodeExist (sectionNode,node.Text)) {
					SortColumnNode newNode = new SortColumnNode (node.Text,
					                                             ExplorerTree.ascendingIcon,
					                                             ExplorerTree.sortColumnMenu);

					newNode.SortDirection = ListSortDirection.Ascending;
					this.SelectedNode = newNode;
					this.CheckNode (newNode);
					sectionNode.Nodes.Add(newNode);
					this.reportModel.ReportSettings.SortColumnCollection.Add(new SortColumn(newNode.Text,
					                                                                        ListSortDirection.Ascending,
					                                                                        typeof(System.String),false));
					this.OnPropertyChanged ("Sort_Group");
				}
			}
		}
		
		#endregion
		
		
		
		private static bool NodeExist (SectionNode sec,string nodeName) 
		{
			if (sec.Nodes.Count > 0) {
				for (int i = 0;i < sec.Nodes.Count ;i++ ) {
					if (sec.Nodes[i].Text == nodeName) {
						return true;
					}
				}
			} 
			return false;
		}
		
		
		private  void CheckNode (TreeNode node)
		{
			SortColumnNode cn = node as SortColumnNode;
			if (cn != null) {
				if (node.Parent == nodeSorting) {
					if (cn.SortDirection ==  ListSortDirection.Ascending) {
						cn.ImageIndex = ascendingIcon;
					} else {
						cn.ImageIndex = descendingIcon;
					}
				} else if (node.Parent == this.nodeGrouping) {
					cn.ImageIndex = clearIcon;
					cn.SelectedImageIndex = clearIcon;
				}
			}
		}
		
		
		#region Update Sorting - Grouping

		private ColumnCollection CollectSortColumns()
		{
			ColumnCollection cl = new ColumnCollection();
			if (this.nodeSorting.Nodes.Count > 0) {
				SortColumn sortColumn;
				AbstractColumn abstrCol;
			
				foreach (SortColumnNode sortNode in this.nodeSorting.Nodes) {
				abstrCol = this.reportModel.ReportSettings.AvailableFieldsCollection.Find(sortNode.Text);
					
					if (abstrCol != null) {
						sortColumn = new SortColumn (sortNode.FieldName,
						                     sortNode.SortDirection,
						                     abstrCol.DataType,true);
					} else {
						sortColumn = new SortColumn (sortNode.Text,
						                     sortNode.SortDirection,
						                     typeof(System.String),true);
					}
					cl.Add(sortColumn);
				}
			}
			return cl;
		}
		
		#endregion
	
		
		#region Treehandling
		
		public void RemoveNode()
		{
			if (this.SelectedNode != null) {
				AbstractColumn abstr = this.reportModel.ReportSettings.SortColumnCollection.Find(this.SelectedNode.Text);
				if (abstr != null) {
					this.reportModel.ReportSettings.SortColumnCollection.Remove(abstr as SortColumn);
				}
				TreeNode parent = this.SelectedNode.Parent;
				this.SelectedNode.Remove();
				this.SelectedNode = parent;
				this.OnPropertyChanged ("RemoveNode");
			}
		}
			
		public void ClearSection ()
		{
			this.nodeSorting.Nodes.Clear();
			this.reportModel.ReportSettings.SortColumnCollection.Clear();
			this.OnPropertyChanged ("ClearSection");
		}
		
		public void ToggleSortOrder() 
		{
			SortColumnNode scn = this.SelectedNode as SortColumnNode;
			
			if (scn != null) {
				if (scn.SortDirection ==  ListSortDirection.Ascending) {
					scn.SortDirection = ListSortDirection.Descending;
					scn.ImageIndex = descendingIcon;
					scn.SelectedImageIndex = descendingIcon;
				} else {
					scn.SortDirection = ListSortDirection.Ascending;
					scn.ImageIndex = ascendingIcon;
					scn.SelectedImageIndex = ascendingIcon;
				}
				SortColumn abstr = (SortColumn)this.reportModel.ReportSettings.SortColumnCollection.Find(this.SelectedNode.Text);
				abstr.SortDirection = scn.SortDirection;
				this.OnPropertyChanged ("ToggleSortOrder");
			}
		}
		
		#endregion
		
		
		#region INotifyPropertyChanged
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private void OnPropertyChanged (string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this,new System.ComponentModel.PropertyChangedEventArgs(property));
			}
		}
		
		#endregion
		
		
		#region FillTree
		
		private void SetSortColumns()
		{
			this.nodeSorting.Nodes.Clear();
			SortColumnNode scn = null;
			foreach (SortColumn sc in this.reportModel.ReportSettings.SortColumnCollection){
				if (sc.SortDirection == ListSortDirection.Ascending) {
					scn  = new SortColumnNode (sc.ColumnName,ascendingIcon,"/SharpDevelopReports/ContextMenu/FieldsExplorer/ColumnTreeNode");
				} else {
					scn = new SortColumnNode (sc.ColumnName,ascendingIcon,"/SharpDevelopReports/ContextMenu/FieldsExplorer/ColumnTreeNode");
				}
				this.nodeSorting.Nodes.Add(scn);
			}
		}
		
		
		private void SetAvailableFields (AbstractColumn af)
		{
			ColumnNode node = new ColumnNode(af.ColumnName,columnIcon);
			node.Tag = this.nodeAvailableFields;
			node.SelectedImageIndex = columnIcon;
			this.nodeAvailableFields.Nodes.Add(node);
		}
		
		
		private void SetParameters ()
		{
			this.nodeParams.Nodes.Clear();		
			foreach (BasicParameter p in this.reportModel.ReportSettings.ParameterCollection)
			{
				string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                         "{0}[{1}]",p.ParameterName,p.ParameterValue);
				ParameterNode node = new ParameterNode(s,columnIcon);
				this.nodeParams.Nodes.Add(node);
			}
		}
		
		
		private void SetFunctions ()
		{
			//ReportStringTagProvider prov = new ReportStringTagProvider();
			//ICSharpCode.Reports.Core.StringParser.RegisterStringTagProvider(prov);
			/*
			foreach (ICSharpCode.Reports.Core.BaseSection baseSection in this.reportModel.SectionCollection) {
				foreach (ICSharpCode.Reports.Core.BaseReportItem item in baseSection.Items) {
					if (item != null) {
						if (ICSharpCode.Reports.Core.StringParser.IsFunction(item)) {
							ICSharpCode.Reports.Core.BaseTextItem t = item as ICSharpCode.Reports.Core.BaseTextItem;
							FunctionNode fn = new FunctionNode(t.Text,functionIcon);
							this.nodeFunction.Nodes.Add(fn);
						}
					}
				}
			}
			*/
		}
		
		
		public  void BuildTree () 
		{
			this.BeginUpdate();
			this.reportModel.ReportSettings.AvailableFieldsCollection.ForEach(SetAvailableFields);
			SetSortColumns();
			SetFunctions();
			SetParameters();
			this.EndUpdate();
		}
		
		#endregion
		
		#region Build Basic Tree
		
		private void BuildNodes()
		{
			BeginUpdate();
			this.Nodes.Clear();
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                         "[{0}]",this.reportModel.ReportSettings.ReportName);
			this.nodeRoot = new SectionNode(s);
			
			this.nodeModel =  new SectionNode("Model");
			
			nodeAvailableFields = new SectionNode(ResourceService.GetString("SharpReport.FieldsExplorer.AvailableFields"));
			nodeAvailableFields.ImageIndex = folderClosed;
			nodeAvailableFields.SelectedImageIndex = folderOpen;
			// we don't like a ContextMenu here
			nodeAvailableFields.ContextMenuAddinTreePath = String.Empty;
			this.nodeModel.Nodes.Add(this.nodeAvailableFields);
			
	
			nodeSorting = new SectionNode (ResourceService.GetString("SharpReport.FieldsExplorer.Sorting"));
			nodeSorting.ImageIndex = folderClosed;
			nodeSorting.SelectedImageIndex = folderOpen;
			nodeSorting.ContextMenuAddinTreePath = ExplorerTree.sectionContextMenu;
			this.nodeModel.Nodes.Add(this.nodeSorting);
			
			nodeGrouping = new SectionNode (ResourceService.GetString("SharpReport.FieldsExplorer.Grouping"));
			nodeGrouping.ImageIndex = folderClosed;
			nodeGrouping.SelectedImageIndex = folderOpen;
			nodeGrouping.ContextMenuAddinTreePath = ExplorerTree.sectionContextMenu;
			this.nodeModel.Nodes.Add(this.nodeGrouping);
			
			nodeFunction = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Functions"));
			nodeFunction.ImageIndex = folderClosed;
			nodeFunction.SelectedImageIndex = folderOpen;
			this.nodeModel.Nodes.Add(this.nodeFunction);
			
			nodeParams = new SectionNode(ResourceService.GetString("SharpReport.FieldsExplorer.Parameters"));
			nodeParams.ImageIndex = folderClosed;
			nodeParams.SelectedImageIndex = folderOpen;
			nodeParams.ContextMenuAddinTreePath = ExplorerTree.parameterEditorMenu;
			
			
			this.nodeModel.Nodes.Add(this.nodeParams);
			this.nodeRoot.Nodes.Add(nodeModel);
			
			this.Nodes.Add (this.nodeRoot);
			this.EndUpdate();
		}
		
		
		private void InitImageList() 
		{
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imageList.Images.Add(new Bitmap(1, 1));

			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SelectionArrow"));
			
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpReport.Ascending"));

			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpReport.Descending"));
			//Table's or procedure
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Table"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Procedure"));
			
			//Parameters
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Column"));
			
			//Function
			imageList.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.SharpReport.Function"));
			this.ImageList = imageList;
		}
		
		#endregion
		
		
		#region Property's
		
		public ReportModel ReportModel {
			set { this.reportModel = value;
				this.BuildNodes();
				this.BuildTree();
			}
		}
		
		
		public ColumnCollection SortColumnCollection
		{
			get{return this.CollectSortColumns();}
		}
		
		#endregion
		
	}
}
