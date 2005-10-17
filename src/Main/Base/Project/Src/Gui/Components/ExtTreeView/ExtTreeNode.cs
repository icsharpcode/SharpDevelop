// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ExtTreeNode : TreeNode, IDisposable, IClipboardHandler
	{
		string contextmenuAddinTreePath = null;
		protected bool isInitialized    = false;
		string  image                    = null;
		
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
		protected bool canLabelEdit = true;
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
		
		List<ExtTreeNode> invisibleNodes = new List<ExtTreeNode>();
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
					((ExtTreeNode)node).Dispose();
				}
			}
		}
		#endregion
		
		#region Drawing routines
		protected bool drawDefault      = true;
		
		public bool DrawDefault {
			get {
				return drawDefault;
			}
		}
		
		protected virtual void DrawBackground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int width = MeasureItemWidth(e);
			Rectangle backRect = new Rectangle(e.Bounds.X, e.Bounds.Y, width, e.Bounds.Height);
			
			if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected) {
				g.FillRectangle(SystemBrushes.Highlight, backRect);
			} else {
				g.FillRectangle(SystemBrushes.Window, backRect);
			}
			
			if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
				backRect.Width--;
				backRect.Height--;
				g.DrawRectangle(SystemPens.HighlightText, backRect);
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
			SizeF size = g.MeasureString(text,  font);
			return (int)size.Width;
		}
		
		protected void DrawText(Graphics g, string text, Brush brush, Font font, ref float x, int y)
		{
			g.DrawString(text, font, brush, new PointF(x, y));
			
			SizeF size = g.MeasureString(text,  font);
			x += size.Width;
		}
		#endregion
		
		#region fonts
		static Font font;
		static Font boldFont;
		static Font italicFont;
		
		static Font monospacedFont;
		static Font boldMonospacedFont;
		static Font italicMonospacedFont;
		
		public static Font BoldMonospacedFont {
			get {
				return boldMonospacedFont;
			}
		}
		public static Font ItalicMonospacedFont {
			get {
				return italicMonospacedFont;
			}
		}
		public static Font MonospacedFont {
			get {
				return monospacedFont;
			}
		}
		
		public static Font Font {
			get {
				return font;
			}
		}
		
		public static Font BoldFont {
			get {
				return boldFont;
			}
		}
		
		public static Font ItalicFont {
			get {
				return italicFont;
			}
		}
		
		static ExtTreeNode()
		{
			font                = new Font("Tahoma", 9);
			boldFont            = new Font("Tahoma", 9, FontStyle.Bold);
			italicFont          = new Font("Tahoma", 9, FontStyle.Italic);
			
			monospacedFont       = new Font("Courier New", 10);
			boldMonospacedFont   = new Font("Courier New", 10, FontStyle.Bold);
			italicMonospacedFont = new Font("Courier New", 10, FontStyle.Italic);
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
		#endregion
	}
}
