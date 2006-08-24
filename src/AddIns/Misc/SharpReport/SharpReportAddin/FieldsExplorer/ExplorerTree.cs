/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 01.08.2006
 * Time: 15:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using SharpReportCore;

namespace SharpReportAddin{
	/// <summary>
	/// Description of ExplorerTree.
	/// </summary>
	internal class ExplorerTree:TreeView{
		private SectionTreeNode nodeRoot;
		private SectionTreeNode nodeModel;
		private SectionTreeNode nodeLayout;
		private SectionTreeNode nodeAvailableFields;
		private SectionTreeNode nodeSorting;
		private SectionTreeNode nodeGrouping;
		private TreeNode nodeFunction;
		private TreeNode nodeParams;
		
		private const int folderClosed = 0;
		private const int folderOpen  = 1;
		private const int clearIcon = 2;
		
		private const int ascendingIcon = 4;
		private const int descendingIcon = 5;
		private const int storedprocIcon = 7;

		private const int columnIcon = 8;
		private const int functionIcon = 9;
		
		public ExplorerTree():base(){
			LabelEdit     = true;
			AllowDrop     = true;
			HideSelection = false;
			Dock          = DockStyle.Fill;
			Scrollable = true;
			this.MouseDown += new MouseEventHandler(TreeMouseDown );
			this.InitImageList();
			BuildNodes();
		}
		
		
		#region Contextmenu
		
		private void TreeMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			
			TreeNode node = this.GetNodeAt(PointToClient(Cursor.Position));
			if (node != null) {
				this.SelectedNode = node;
				CheckNode (node);
				if (e.Button == MouseButtons.Right) {
					AbstractFieldsNode abstrNode = node as AbstractFieldsNode;
					if (abstrNode != null) {
						if (abstrNode.ContextmenuAddinTreePath.Length > 0) {
							ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,abstrNode.ContextmenuAddinTreePath);
							ctMen.Show (this,new Point (e.X,e.Y));
						}
					}
				}
			}
		}
		
		public void ClearSortNode() {
			this.nodeSorting.Nodes.Clear();
		}
		
		public  bool IsGroupNode (SectionTreeNode node) {
			return (node == this.nodeGrouping);
		}
		
		public static bool CheckForExist (SectionTreeNode sec,ColumnsTreeNode col) {
			if (sec.Nodes.Count > 0) {
				for (int i = 0;i < sec.Nodes.Count ;i++ ) {
					if (sec.Nodes[i].Text == col.Text) {
						return true;
					}
				}
			} else {
				return false;
			}
			return false;
		}
		
		public  void CheckNode (TreeNode node) {
			ColumnsTreeNode cn = node as ColumnsTreeNode;
			
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
		#endregion
		
		#region Update ReportModel
		
		private void UpdateSorting (ReportModel model) {
			model.ReportSettings.SortColumnCollection.Clear();
			if (this.nodeSorting.Nodes.Count > 0) {
				SortColumn sc;
				AbstractColumn af;
				foreach (ColumnsTreeNode cn in this.nodeSorting.Nodes) {
					af = model.ReportSettings.AvailableFieldsCollection.Find(cn.Text);
					if (af != null) {
						sc = new SortColumn (cn.Text,
						                     cn.SortDirection,
						                     af.DataType);
					} else {
						sc = new SortColumn (cn.Text,
						                     cn.SortDirection,
						                     typeof(System.String));
					}
					model.ReportSettings.SortColumnCollection.Add(sc);
				}
			}
		}
		
		
		private void UpdateGrouping (ReportModel model) {
			model.ReportSettings.GroupColumnsCollection.Clear();
			if (this.nodeGrouping.Nodes.Count > 0) {
				GroupColumn gc;
				for (int i = 0;i < this.nodeGrouping.Nodes.Count ;i++ ) {
					ColumnsTreeNode cn = (ColumnsTreeNode)this.nodeGrouping.Nodes[i];
					gc = new GroupColumn (cn.Text,i,cn.SortDirection);
					model.ReportSettings.GroupColumnsCollection.Add(gc);
				}
			}
		}
		
		#endregion
		
		#region Commands
		
		public void CollectModel (ReportModel model) {
			UpdateSorting(model);
			UpdateGrouping(model);
		}
		
		public void ClearAndFill() {
			this.Nodes.Clear();
			this.BuildNodes();
		}
		
		public void ToggleOrder() {
			if (this.SelectedNode is ColumnsTreeNode) {
				ColumnsTreeNode cn = (ColumnsTreeNode)this.SelectedNode;
				if (cn.SortDirection ==  ListSortDirection.Ascending) {
					cn.SortDirection = ListSortDirection.Descending;
					cn.ImageIndex = descendingIcon;
					cn.SelectedImageIndex = descendingIcon;
				} else {
					cn.SortDirection = ListSortDirection.Ascending;
					cn.ImageIndex = ascendingIcon;
					cn.SelectedImageIndex = ascendingIcon;
				}
				
			}
		}
		#endregion
		
		public int FolderClosed {
			get {
				return folderClosed;
			}
			
		}
		
		public int FolderOpen {
			get {
				return folderOpen;
			}
		}
		
		public int ClearIcon {
			get {
				return clearIcon;
			}
		}
		
		public int AscendingIcon {
			get {
				return ascendingIcon;
			}
		}
		
		public int DescendingIcon {
			get {
				return descendingIcon;
			}
		}
		
		public int StoredprocIcon {
			get {
				return storedprocIcon;
			}
		}
		
		public int ColumnIcon {
			get {
				return columnIcon;
			}
		}
		
		public int FunctionIcon {
			get {
				return functionIcon;
			}
		}
		
		
		#region FillTree
		
		private void SetAvailableFields (ReportModel model) {
			
			this.nodeAvailableFields.Nodes.Clear();
			
			
			foreach (AbstractColumn af in model.ReportSettings.AvailableFieldsCollection){
				
				ColumnsTreeNode node = new ColumnsTreeNode(af.ColumnName);
				node.Tag = this.nodeAvailableFields;
				
				//we don't like ContextMenu here
				node.ContextmenuAddinTreePath = "";
				// and a node is a node, otherwise we cant't dragdrop
				node.ImageIndex = columnIcon;
				node.SelectedImageIndex = columnIcon;
				
				this.nodeAvailableFields.Nodes.Add(node);
			}

		}
		
		
		void SetSortFields(ColumnCollection collection){
			ColumnsTreeNode node;
			
			this.nodeSorting.Nodes.Clear();

			foreach (SortColumn sc in collection) {
				node = new ColumnsTreeNode(sc.ColumnName,sc.SortDirection);
				if (node.SortDirection == ListSortDirection.Ascending) {
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
				} else {
					node.ImageIndex = descendingIcon;
					node.SelectedImageIndex = descendingIcon;
				}
				this.nodeSorting.Nodes.Add(node);
			}
		}
		
		private void SetGroupFields(ColumnCollection collection){
			ColumnsTreeNode node;
			
			this.nodeGrouping.Nodes.Clear();
			foreach (GroupColumn gc in collection) {
				node = new ColumnsTreeNode(gc.ColumnName);
				if (node.SortDirection == ListSortDirection.Ascending) {
					node.ImageIndex = ascendingIcon;
					node.SelectedImageIndex = ascendingIcon;
				} else {
					node.ImageIndex = descendingIcon;
					node.SelectedImageIndex = descendingIcon;
				}
				this.nodeGrouping.Nodes.Add(node);
			}
		}
		
		private void SetParamFields (SqlParametersCollection collection){
			ColumnsTreeNode node;
			
			this.nodeParams.Nodes.Clear();
			foreach (SqlParameter par in collection) {
				StringBuilder sb = new StringBuilder(par.ParameterName);
				sb.Append(" {");
				sb.Append(par.DataType);
				sb.Append("}");
				node = new ColumnsTreeNode(sb.ToString());
				node.Tag = par;
				// No ContextMenu for Parameters
				node.ContextmenuAddinTreePath = String.Empty;
				node.SelectedImageIndex = columnIcon;
				node.ImageIndex = columnIcon;
				this.nodeParams.Nodes.Add (node);
			}
		}
		
		void SetFunctions(ReportSectionCollection collection){
			AbstractFieldsNode node;
			this.nodeFunction.Nodes.Clear();
			foreach (SharpReport.ReportSection section in collection) {
				foreach (BaseReportObject item in section.Items) {
					BaseFunction func = item as BaseFunction;
					if (func != null) {
						node = new ColumnsTreeNode (ResourceService.GetString(func.LocalisedName));
						// No ContextMenu for Functions included in the Report
						node.ContextmenuAddinTreePath = String.Empty;
						node.SelectedImageIndex = functionIcon;
						node.ImageIndex = functionIcon;
						this.nodeFunction.Nodes.Add(node);
					}
				}
			}
		}
		
		public void FillTree (ReportModel model) {
			this.BeginUpdate();
			try {
				SetAvailableFields(model);
				SetGroupFields(model.ReportSettings.GroupColumnsCollection);
				SetSortFields(model.ReportSettings.SortColumnCollection);
				SetParamFields (model.ReportSettings.SqlParametersCollection);
				SetFunctions(model.SectionCollection);
				this.EndUpdate();
			} catch (Exception ) {
				throw;
			}
		}
		
		#endregion
		
		#region Build Basic Tree
		
		private void BuildNodes() {
			BeginUpdate();
			this.Nodes.Clear();
			this.nodeRoot = new SectionTreeNode("Report");
			
			this.nodeModel =  new SectionTreeNode("Model");
			
			nodeAvailableFields = new SectionTreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.AvailableFields"));
			nodeAvailableFields.ImageIndex = folderClosed;
			nodeAvailableFields.SelectedImageIndex = folderOpen;
			// we don't like a ContextMenu here
			nodeAvailableFields.ContextmenuAddinTreePath = "";
			this.nodeModel.Nodes.Add(this.nodeAvailableFields);
			
	
			nodeSorting = new SectionTreeNode (ResourceService.GetString("SharpReport.FieldsExplorer.Sorting"));
			nodeSorting.ImageIndex = folderClosed;
			nodeSorting.SelectedImageIndex = folderOpen;
			this.nodeModel.Nodes.Add(this.nodeSorting);
			
			nodeGrouping = new SectionTreeNode (ResourceService.GetString("SharpReport.FieldsExplorer.Grouping"));
			nodeGrouping.ImageIndex = folderClosed;
			nodeGrouping.SelectedImageIndex = folderOpen;
			this.nodeModel.Nodes.Add(this.nodeGrouping);
			
			nodeFunction = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Functions"));
			nodeFunction.ImageIndex = folderClosed;
			nodeFunction.SelectedImageIndex = folderOpen;
			this.nodeModel.Nodes.Add(this.nodeFunction);
			
			nodeParams = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Parameters"));
			nodeParams.ImageIndex = folderClosed;
			nodeParams.SelectedImageIndex = folderOpen;
			
			
			
			this.nodeModel.Nodes.Add(this.nodeParams);
			
			this.nodeRoot.Nodes.Add(nodeModel);
			
			
			this.nodeLayout =  new SectionTreeNode("Layout");
			
			this.nodeRoot.Nodes.Add(nodeLayout);
			Nodes.Add (this.nodeRoot);
			this.EndUpdate();
		}
		
		void InitImageList() {
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
			imageList.Images.Add(ResourceService.GetIcon("Icons.16x16.SharpReport.Function"));
			this.ImageList = imageList;
		}
		
		#endregion
		
	}
}
