using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ExtTreeView.
	/// </summary>
	public class ExtTreeView : TreeView
	{
		Dictionary<string, int> imageIndexTable = new Dictionary<string, int>();
		List<ExtTreeNode> cutNodes = new List<ExtTreeNode>();
		bool isSorted = true;
		
		public bool IsSorted {
			get {
				return isSorted;
			}
			set {
				isSorted = value;
			}
		}
		public List<ExtTreeNode> CutNodes {
			get {
				return cutNodes;
			}
		}
		
		public ExtTreeView()
		{
			DrawMode      = TreeViewDrawMode.OwnerDrawText;
			HideSelection = false;
			AllowDrop     = true;
			this.TreeViewNodeSorter  = new ExtTreeViewComparer();
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			ImageList newImageList = new ImageList();
			newImageList.ImageSize = new Size(16, 16);
			newImageList.ColorDepth = ColorDepth.Depth32Bit;
			this.ImageList = newImageList;
		}
		
		public void SortNodes(TreeNode node)
		{
			if (!isSorted) {
				return;
			}
			if (node == null) {
				foreach (TreeNode childNode in Nodes) {
					SortNodes(childNode);
				}
				return;
			}
			TreeNode[] nodeArray = new TreeNode[node.Nodes.Count];
			node.Nodes.CopyTo(nodeArray, 0);
			Array.Sort(nodeArray, TreeViewNodeSorter);
			node.Nodes.Clear();
			node.Nodes.AddRange(nodeArray);
			
			foreach (TreeNode childNode in nodeArray) {
				SortNodes(childNode);
			}
		}
		
		public void ClearCutNodes()
		{
			foreach (ExtTreeNode node in CutNodes) {
				node.DoPerformCut = false;
			}
			CutNodes.Clear();
		}
		
		public void Clear()
		{
			if (this.IsDisposed) {
				return;
			}
			TreeNode[] nodeArray = new TreeNode[Nodes.Count];
			Nodes.CopyTo(nodeArray, 0);
			Nodes.Clear();
			foreach (TreeNode node in nodeArray) {
				if (node is IDisposable) {
					((IDisposable)node).Dispose();
				}
			}
		}
		
		void ActivateSelectedItem()
		{
			ExtTreeNode node = SelectedNode as ExtTreeNode;
			if (node != null) {
				node.ActivateItem();
			}
		}
		#region label editing
		
		public void StartLabelEdit(ExtTreeNode node)
		{
			if (node == null) {
				return;
			}
			node.EnsureVisible();
			SelectedNode = node;
			LabelEdit = true;
			node.BeforeLabelEdit();
			node.BeginEdit();
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.F2:
					StartLabelEdit(SelectedNode as ExtTreeNode);
					break;
			}
			return base.ProcessDialogKey(keyData);
		}
		
		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			LabelEdit    = false;
			e.CancelEdit = true;
			
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.AfterLabelEdit(e.Label);
			}
			SortNodes(e.Node.Parent);
		}
		#endregion
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			if (e.Node is ExtTreeNode) {
				((ExtTreeNode)e.Node).Expanding();
			}
			SortNodes(e.Node);
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			if (e.Node is ExtTreeNode) {
				((ExtTreeNode)e.Node).Collapsing();
			}
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r') {
				ActivateSelectedItem();
			}
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			ActivateSelectedItem();
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			TreeNode node = GetNodeAt(e.X, e.Y);
			if (node != null) {
				if (SelectedNode != node) {
					SelectedNode = node;
				}
			} else {
				SelectedNode = null;
				this.ContextMenuStrip = null;
			}
		}
		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				this.ContextMenuStrip = MenuService.CreateContextMenu(e.Node, node.ContextmenuAddinTreePath);
			}
		}
		
		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			base.OnAfterCheck(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.CheckedChanged();
			}
		}
		
		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null && !node.DrawDefault) {
				node.Draw(e);
				e.DrawDefault = false;
			} else {
				e.DrawDefault = true;
			}
			base.OnDrawNode(e);
		}
		
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			ExtTreeNode node = e.Item as ExtTreeNode;
			if (node != null) {
				DataObject dataObject = node.DragDropDataObject;
				if (dataObject != null) {
					DoDragDrop(dataObject, DragDropEffects.All);
					SortNodes(node.Parent);
				}
			}
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				DragDropEffects effect = DragDropEffects.None;
				
				if ((e.KeyState & 8) > 0) { // CTRL key pressed.
					effect = DragDropEffects.Copy;
				} else {
					effect = DragDropEffects.Move;
				}
				e.Effect = node.GetDragDropEffect(e.Data, effect);
				
				if (e.Effect != DragDropEffects.None) {
					SelectedNode = node;
				}
			}
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				node.DoDragDrop(e.Data, e.Effect);
				SortNodes(node.Parent);
			}
		}
		
		public int GetImageIndexForImage(string image, bool performCutBitmap)
		{
			string imageKey = performCutBitmap ? (image + "_ghost") : image;
			if (!imageIndexTable.ContainsKey(imageKey)) {
				ImageList.Images.Add(performCutBitmap ? IconService.GetGhostBitmap(image) : IconService.GetBitmap(image));
				imageIndexTable[imageKey] = ImageList.Images.Count - 1;
				return ImageList.Images.Count - 1;
			}
			return imageIndexTable[imageKey];
		}
	}
}
