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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	using TreeView = System.Windows.Forms.TreeView;
	
	public class ExtTreeNode : TreeNode, IDisposable, IClipboardHandler
	{
		string contextmenuAddinTreePath = null;
		protected bool isInitialized    = false;
		string  image                    = null;
		
		public bool IsInitialized {
			get {
				return isInitialized;
			}
		}
		
		public virtual string ContextmenuAddinTreePath {
			get {
				return contextmenuAddinTreePath;
			}
			set {
				contextmenuAddinTreePath = value;
			}
		}
		
		public void SetIcon(string iconName)
		{
			if (iconName == null) {
				return;
			}
			this.image = iconName;
			
			ExtTreeView tree = TreeView as ExtTreeView;
			if (tree != null) {
				int index = tree.GetImageIndexForImage(iconName, DoPerformCut);
				if (ImageIndex != index) {
					ImageIndex = SelectedImageIndex = index;
				}
			}
		}
		
		TreeNode internalParent;
		
		public new TreeNode Parent {
			get {
				return internalParent;
			}
		}
		
		public void AddTo(TreeNode node)
		{
			internalParent = node;
			AddTo(node.Nodes);
		}
		public void AddTo(TreeView view)
		{
			internalParent = null;
			AddTo(view.Nodes);
		}
		
		public void Insert(int index, TreeNode parentNode)
		{
			internalParent = parentNode;
			parentNode.Nodes.Insert(index, this);
			Refresh();
		}
		
		public void Insert(int index, TreeView view)
		{
			internalParent = null;
			view.Nodes.Insert(index, this);
			Refresh();
		}
		
		void AddTo(TreeNodeCollection nodes)
		{
			nodes.Add(this);
			Refresh();
		}
		
		protected virtual void Initialize()
		{
		}
		public void PerformInitialization()
		{
			if (!isInitialized) {
				Initialize();
				isInitialized = true;
			}
		}
		
		public virtual void Expanding()
		{
			PerformInitialization();
		}
		
		public virtual void Collapsing()
		{
		}
		
		public virtual void ActivateItem()
		{
			this.Toggle();
		}
		
		public virtual void CheckedChanged()
		{
		}
		
		public virtual void Refresh()
		{
			SetIcon(image);
			foreach (TreeNode node in Nodes) {
				if (node is ExtTreeNode) {
					((ExtTreeNode)node).Refresh();
				}
			}
		}
		
		#region Label edit
		protected bool canLabelEdit = false;
		public virtual bool CanLabelEdit {
			get {
				return canLabelEdit;
			}
		}
		
		/// <summary>
		/// This method is before a label edit starts.
		/// </summary>
		public virtual void BeforeLabelEdit()
		{
		}
		
		/// <summary>
		/// This method is called when a label edit has finished.
		/// The New Name is the 'new name' the user has given the node. The
		/// node must handle the name change itself.
		/// </summary>
		public virtual void AfterLabelEdit(string newName)
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Visibility
		public virtual bool Visible {
			get {
				return true;
			}
		}
		
		public IEnumerable<ExtTreeNode> AllNodes {
			get {
				foreach (ExtTreeNode n in Nodes) {
					yield return n;
				}
				foreach (ExtTreeNode n in invisibleNodes) {
					yield return n;
				}
			}
		}
		
		protected List<ExtTreeNode> invisibleNodes = new List<ExtTreeNode>();
		public virtual void UpdateVisibility()
		{
			for (int i = 0; i < invisibleNodes.Count;) {
				if (invisibleNodes[i].Visible) {
					invisibleNodes[i].AddTo(this);
					invisibleNodes.RemoveAt(i);
					continue;
				}
				++i;
			}
			
			foreach (TreeNode node in Nodes) {
				if (node is ExtTreeNode) {
					ExtTreeNode extTreeNode = (ExtTreeNode)node;
					if (!extTreeNode.Visible) {
						invisibleNodes.Add(extTreeNode);
					}
				}
			}
			
			foreach (TreeNode node in invisibleNodes) {
				Nodes.Remove(node);
			}
			
			foreach (TreeNode node in Nodes) {
				if (node is ExtTreeNode) {
					((ExtTreeNode)node).UpdateVisibility();
				}
			}
		}
		#endregion
		
		#region System.IDisposable interface implementation
		bool isDisposed = false;
		
		public bool IsDisposed {
			get {
				return isDisposed;
			}
		}
		
		public virtual void Dispose()
		{
			isDisposed = true;
			foreach (TreeNode node in Nodes) {
				if (node is IDisposable) {
					((IDisposable)node).Dispose();
				}
			}
		}
		#endregion
		
		#region Drawing routines
		protected bool drawDefault = true;
		
		public bool DrawDefault {
			get {
				return drawDefault;
			}
		}
		
		protected virtual void DrawBackground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int width = MeasureItemWidth(e) + 2;
			Rectangle backRect = new Rectangle(e.Bounds.X, e.Bounds.Y, width, e.Bounds.Height);
			
			if ((e.State & (TreeNodeStates.Selected | TreeNodeStates.Focused)) == TreeNodeStates.Selected) {
				g.FillRectangle(SystemBrushes.Control, backRect);
			} else if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected) {
				g.FillRectangle(SystemBrushes.Highlight, backRect);
			} else {
				g.FillRectangle(SystemBrushes.Window, backRect);
			}
			
			if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
				backRect.Width--;
				backRect.Height--;
				using (Pen dottedPen = new Pen(SystemColors.WindowText)) {
					dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
					g.DrawRectangle(dottedPen, backRect);
					Color h = SystemColors.Highlight;
					dottedPen.Color = Color.FromArgb(255 - h.R, 255 - h.G, 255 - h.B);
					dottedPen.DashOffset = 1;
					g.DrawRectangle(dottedPen, backRect);
				}
				g.DrawLine(SystemPens.WindowText, backRect.Right + 1, backRect.Y, backRect.Right + 1, backRect.Bottom);
			}
		}
		
		protected virtual int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			return MeasureTextWidth(e.Graphics, Text, TreeView.Font);
		}
		
		protected virtual void DrawForeground(DrawTreeNodeEventArgs e)
		{
		}
		
		public void Draw(DrawTreeNodeEventArgs e)
		{
			DrawBackground(e);
			DrawForeground(e);
		}
		
		// Helper routines
		protected int MeasureTextWidth(Graphics g, string text, Font font)
		{
			SizeF size = g.MeasureString(text, font);
			return (int)size.Width;
		}
		
		const TreeNodeStates SelectedAndFocused = TreeNodeStates.Selected | TreeNodeStates.Focused;
		
		protected void DrawText(DrawTreeNodeEventArgs e, string text, Brush brush, Font font)
		{
			float x = e.Bounds.X;
			DrawText(e, text, brush, font, ref x);
		}
		
		protected void DrawText(DrawTreeNodeEventArgs e, string text, Brush brush, Font font, ref float x)
		{
			if ((e.State & SelectedAndFocused) == SelectedAndFocused) {
				brush = SystemBrushes.HighlightText;
			}
			e.Graphics.DrawString(text, font, brush, new PointF(x, e.Bounds.Y));
			
			SizeF size = e.Graphics.MeasureString(text, font);
			x += size.Width;
		}
		
		protected Color GetTextColor(TreeNodeStates state, Color c)
		{
			if ((state & SelectedAndFocused) == SelectedAndFocused) {
				return SystemColors.HighlightText;
			}
			return c;
		}
		#endregion
		
		#region fonts
		static Font regularBigFont, boldBigFont, italicBigFont;
		static Font boldMonospacedFont, italicMonospacedFont;
		static Font boldDefaultFont, italicDefaultFont;
		
		public static Font RegularMonospacedFont {
			get {
				return SD.WinForms.DefaultMonospacedFont;
			}
		}
		public static Font BoldMonospacedFont {
			get {
				return boldMonospacedFont
					?? (boldMonospacedFont = SD.WinForms.LoadDefaultMonospacedFont(FontStyle.Bold));
			}
		}
		public static Font ItalicMonospacedFont {
			get {
				return italicMonospacedFont
					?? (italicMonospacedFont = SD.WinForms.LoadDefaultMonospacedFont(FontStyle.Italic));
			}
		}
		
		public static Font RegularDefaultFont {
			get {
				return TreeView.DefaultFont;
			}
		}
		
		public static Font BoldDefaultFont {
			get {
				return boldDefaultFont
					?? (boldDefaultFont = SD.WinForms.LoadFont(RegularDefaultFont, FontStyle.Bold));
			}
		}
		
		public static Font ItalicDefaultFont {
			get {
				return italicDefaultFont
					?? (italicDefaultFont = SD.WinForms.LoadFont(RegularDefaultFont, FontStyle.Italic));
			}
		}
		
		public static Font RegularBigFont {
			get {
				return regularBigFont
					?? (regularBigFont = SD.WinForms.LoadFont("Tahoma", 9));
			}
		}
		
		public static Font BoldBigFont {
			get {
				return boldBigFont
					?? (boldBigFont = SD.WinForms.LoadFont("Tahoma", 9, FontStyle.Bold));
			}
		}
		
		public static Font ItalicBigFont {
			get {
				return italicBigFont
					?? (italicBigFont = SD.WinForms.LoadFont("Tahoma", 9, FontStyle.Italic));
			}
		}
		#endregion
		
		#region Drag and Drop
		/// <summary>
		/// Generates a Drag &amp; Drop data object. If this property returns null
		/// the node indicates that it can't be dragged.
		/// </summary>
		public virtual DataObject DragDropDataObject {
			get {
				return null;
			}
		}
		
		/// <summary>
		/// Gets the drag &amp; drop effect, when a DataObject is dragged over this node.
		/// </summary>
		/// <param name="dataObject"></param>
		/// <param name="proposedEffect">
		/// The default effect DragDropEffects.Copy and DragDropEffects.Move, depending on the
		/// key the user presses while performing d&amp;d.
		/// </param>
		/// <returns>
		/// DragDropEffects.None when no drag&amp;drop can occur.
		/// </returns>
		public virtual DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			return DragDropEffects.None;
		}
		
		/// <summary>
		/// If GetDragDropEffect returns something != DragDropEffects.None this method should
		/// handle the DoDragDrop(obj, GetDragDropEffect(obj, proposedEffect)).
		/// </summary>
		public virtual void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		
		bool doPerformCut = false;
		public virtual bool DoPerformCut {
			get {
				ExtTreeNode parent = Parent as ExtTreeNode;
				return parent == null ? doPerformCut : doPerformCut | parent.DoPerformCut;
			}
			set {
				this.doPerformCut = value;
				if (this.doPerformCut) {
					((ExtTreeView)TreeView).CutNodes.Add(this);
				}
				Refresh();
			}
		}
		
		public virtual bool EnableCut {
			get {
				return false;
			}
		}
		
		public virtual bool EnableCopy {
			get {
				return false;
			}
		}
		
		public virtual bool EnablePaste {
			get {
				return false;
			}
		}
		
		public virtual bool EnableDelete {
			get {
				return false;
			}
		}
		
		public virtual bool EnableSelectAll {
			get {
				return false;
			}
		}
		
		public virtual void Cut()
		{
			throw new System.NotImplementedException();
		}
		
		public virtual void Copy()
		{
			throw new System.NotImplementedException();
		}
		
		public virtual void Paste()
		{
			throw new System.NotImplementedException();
		}
		
		public virtual void Delete()
		{
			throw new System.NotImplementedException();
		}
		
		public virtual void SelectAll()
		{
			throw new System.NotImplementedException();
		}
		#endregion
		
		#region sorting
		protected int sortOrder = 0;
		public virtual int SortOrder {
			get {
				return sortOrder;
			}
		}
		
		public virtual string CompareString {
			get {
				return Text;
			}
		}
		
		int GetInsertionIndex(TreeNodeCollection nodes, TreeView treeView)
		{
			if (treeView == null) {
				return nodes.Count;
			}
			
			Comparison<TreeNode> comparison = null;
			
			ExtTreeView etv = treeView as ExtTreeView;
			if (etv == null) {
				if (!treeView.Sorted) {
					return nodes.Count;
				}
				if (treeView.TreeViewNodeSorter != null) {
					comparison = treeView.TreeViewNodeSorter.Compare;
				}
			} else {
				if (!etv.IsSorted) {
					return nodes.Count;
				}
				if (etv.NodeSorter != null) {
					comparison = etv.NodeSorter.Compare;
				}
			}
			
			if (comparison == null) {
				return nodes.Count;
			}
			
			for (int i = 0; i < nodes.Count; ++i) {
				if (comparison(this, nodes[i]) < 0) {
					return i;
				}
			}
			
			return nodes.Count;
		}
		
		/// <summary>
		/// Inserts this node into the specified TreeView at the position
		/// determined by the comparer of the TreeView, assuming that
		/// all other immediate child nodes of the TreeView are in sorted order.
		/// </summary>
		public void InsertSorted(TreeView treeView)
		{
			this.Insert(this.GetInsertionIndex(treeView.Nodes, treeView), treeView);
		}
		
		/// <summary>
		/// Inserts this node into the specified <paramref name="parentNode"/>
		/// at the position determined by the comparer
		/// of the TreeView which contains the <paramref name="parentNode"/>,
		/// assuming that all other immediate child nodes of the <paramref name="parentNode"/>
		/// are in sorted order.
		/// </summary>
		public void InsertSorted(TreeNode parentNode)
		{
			this.Insert(this.GetInsertionIndex(parentNode.Nodes, parentNode.TreeView), parentNode);
		}
		#endregion
	}
}
