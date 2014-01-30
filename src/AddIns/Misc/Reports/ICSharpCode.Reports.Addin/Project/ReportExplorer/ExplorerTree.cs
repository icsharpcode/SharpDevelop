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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reports.Core.BaseClasses;
namespace ICSharpCode.Reports.Addin
{
	
	/// <summary>
	/// Description of ExplorerTree.
	/// </summary>
	internal class ExplorerTree:TreeView,INotifyPropertyChanged
	{
		private const string sortColumnMenuPath = "/SharpDevelopReports/ContextMenu/FieldsExplorer/ColumnSortTreeNode";
		private const string sectionContextMenu = "/SharpDevelopReports/ContextMenu/FieldsExplorer/SectionTreeNode";
		private const string parameterEditorMenu = "/SharpDevelopReports/ContextMenu/FieldsExplorer/ParameterNode";
		private const string groupContextMenuPath ="/SharpDevelopReports/ContextMenu/FieldsExplorer/ColumnGroupTreeNode";
		
		private SectionNode nodeRoot;
		private SectionNode nodeModel;

		private SectionNode nodeAvailableFields;
		private SectionNode nodeSorting;
		private SectionNode nodeGrouping;
		private TreeNode nodeFunction;
		private SectionNode nodeParams;
		
		private static int folderClosed = 0;
		private static int folderOpen  = 1;
//		private static int clearIcon = 2;
		
		private static int ascendingIcon = 4;
		private static int descendingIcon = 5;
//		private static int storedprocIcon = 7;

		private static int columnIcon = 8;
		
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
			if ((sectionNode != null) && (!ExplorerTree.NodeExist (sectionNode,node.Text))) {
				if (sectionNode == this.nodeGrouping) {
					this.nodeSorting.Nodes.Clear();
					AddGrouping(sectionNode,node);
				} else {
					AddSorting (sectionNode,node);
				}
			}
		}
		
		
		private void AddSorting(SectionNode sectionNode, ColumnNode node)
		{
			SortColumnNode sortNode = new SortColumnNode (node.Text,
			                                              ExplorerTree.ascendingIcon,
			                                              ExplorerTree.sortColumnMenuPath);

			sortNode.SortDirection = ListSortDirection.Ascending;
			this.SelectedNode = sortNode;
			sectionNode.Nodes.Add(sortNode);
			this.reportModel.ReportSettings.SortColumnsCollection.Add(new SortColumn(sortNode.Text,
			                                                                         ListSortDirection.Ascending,
			                                                                         typeof(System.String),false));
			this.OnPropertyChanged ("Sorting");
			
		}
		
		
		private void AddGrouping(SectionNode sectionNode,ColumnNode node)
		{
			this.nodeSorting.Nodes.Clear();
			GroupColumnNode groupNode = new GroupColumnNode(node.Text,ExplorerTree.ascendingIcon,
			                                                ExplorerTree.sortColumnMenuPath);
			groupNode.SortDirection = ListSortDirection.Ascending;
			this.SelectedNode = groupNode;
			sectionNode.Nodes.Add(groupNode);
			this.reportModel.ReportSettings.GroupColumnsCollection.Add(new GroupColumn(groupNode.Text, 1,ListSortDirection.Ascending));
			this.OnPropertyChanged ("Grouping");
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
		
		public void RemoveSortNode()
		{
			if (this.SelectedNode != null) {
				AbstractColumn abstr = this.reportModel.ReportSettings.SortColumnsCollection.Find(this.SelectedNode.Text);
				if (abstr != null) {
					this.reportModel.ReportSettings.SortColumnsCollection.Remove(abstr as SortColumn);
				}
				TreeNode parent = this.SelectedNode.Parent;
				this.SelectedNode.Remove();
				this.SelectedNode = parent;
				this.OnPropertyChanged ("RemoveSortNode");
			}
		}
			
		public void RemoveGroupNode ()
		{
			if (this.SelectedNode != null) {
				AbstractColumn abstr = this.reportModel.ReportSettings.GroupColumnsCollection.Find(this.SelectedNode.Text);
				if (abstr != null) {
					this.reportModel.ReportSettings.GroupColumnsCollection.Remove(abstr as GroupColumn);
					TreeNode parent = this.SelectedNode.Parent;
					this.SelectedNode.Remove();
					this.SelectedNode = parent;
					this.OnPropertyChanged ("RemoveGroupNode");
				}
			}
		}
			
		public void ClearSection ()
		{
			this.nodeSorting.Nodes.Clear();
			this.reportModel.ReportSettings.SortColumnsCollection.Clear();
			this.OnPropertyChanged ("ClearSection");
		}
		
		
		public void ToggleSortOrder()
		{
			SortColumnNode sortColumnNode = this.SelectedNode as SortColumnNode;
			
			if (sortColumnNode != null) {
				if (sortColumnNode.SortDirection ==  ListSortDirection.Ascending) {
					sortColumnNode.SortDirection = ListSortDirection.Descending;
					sortColumnNode.ImageIndex = descendingIcon;
					sortColumnNode.SelectedImageIndex = descendingIcon;
				} else {
					sortColumnNode.SortDirection = ListSortDirection.Ascending;
					sortColumnNode.ImageIndex = ascendingIcon;
					sortColumnNode.SelectedImageIndex = ascendingIcon;
				}
				SortColumn abstractColumn = null;
				if (this.SelectedNode is GroupColumnNode) {
					
					abstractColumn = (SortColumn)this.reportModel.ReportSettings.GroupColumnsCollection.Find(this.SelectedNode.Text);
				} else {
					abstractColumn = (SortColumn)this.reportModel.ReportSettings.SortColumnsCollection.Find(this.SelectedNode.Text);
				}
				abstractColumn.SortDirection = sortColumnNode.SortDirection;
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
	
		private void SetGroupColumns()
		{
			foreach (GroupColumn groupColumn in this.reportModel.ReportSettings.GroupColumnsCollection)
			{
				var groupNode  = new GroupColumnNode(groupColumn.ColumnName,groupContextMenuPath);
				if (groupColumn.SortDirection  == ListSortDirection.Ascending) {
					groupNode.ImageIndex  = ascendingIcon;
				} else {
					groupNode.ImageIndex = descendingIcon;
				}
				this.nodeGrouping.Nodes.Add(groupNode);
				foreach (var p in this.reportModel.ReportSettings.AvailableFieldsCollection.Where(p => (p.ColumnName != groupColumn.ColumnName))) 
				{
					var cn = new ColumnNode(p.ColumnName,columnIcon);
					groupNode.Nodes.Add(cn);
				}
			}
		}
			
		
		public  void BuildTree () 
		{
			this.BeginUpdate();
			this.nodeAvailableFields.Nodes.Clear();
			this.nodeSorting.Nodes.Clear();
			this.nodeGrouping.Nodes.Clear();
			this.nodeParams.Nodes.Clear();	
			this.reportModel.ReportSettings.AvailableFieldsCollection.ForEach(SetAvailableFields);
			this.reportModel.ReportSettings.SortColumnsCollection.ForEach(SetSortColumns);
			SetGroupColumns();
			this.reportModel.ReportSettings.ParameterCollection.ForEach(SetParams);
//			SetFunctions();
			this.ExpandAll();
			this.EndUpdate();
		}
		
		
		private void SetAvailableFields (AbstractColumn af)
		{
			ColumnNode node = new ColumnNode(af.ColumnName,columnIcon);
			node.Tag = this.nodeAvailableFields;
			node.SelectedImageIndex = columnIcon;
			this.nodeAvailableFields.Nodes.Add(node);
		}
		
		
		private void SetSortColumns (AbstractColumn column)
		{
			SortColumn sortColumn = column as SortColumn;
			
			if (sortColumn != null) {
				var  sortColumnNode = new SortColumnNode (sortColumn.ColumnName,sortColumnMenuPath);

				if (sortColumn.SortDirection == ListSortDirection.Ascending) {
					sortColumnNode.ImageIndex = ascendingIcon;
				} else {
					sortColumnNode.ImageIndex = descendingIcon;
				}
				this.nodeSorting.Nodes.Add(sortColumnNode);
			}
			
		}
		
		private void SetParams (BasicParameter p)
		{
				string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                         "{0}[{1}]",p.ParameterName,p.ParameterValue);
				ParameterNode node = new ParameterNode(s,columnIcon);
				this.nodeParams.Nodes.Add(node);
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
			
			nodeGrouping = new SectionNode (ResourceService.GetString("SharpReport.FieldsExplorer.Grouping"));
			nodeGrouping.ImageIndex = folderClosed;
			nodeGrouping.SelectedImageIndex = folderOpen;
			nodeGrouping.ContextMenuAddinTreePath = ExplorerTree.sectionContextMenu;
			this.nodeModel.Nodes.Add(this.nodeGrouping);
			
			nodeSorting = new SectionNode (ResourceService.GetString("SharpReport.FieldsExplorer.Sorting"));
			nodeSorting.ImageIndex = folderClosed;
			nodeSorting.SelectedImageIndex = folderOpen;
			nodeSorting.ContextMenuAddinTreePath = ExplorerTree.sectionContextMenu;
			this.nodeModel.Nodes.Add(this.nodeSorting);
			
			
			
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
	
		
		#endregion
		
	}
}
