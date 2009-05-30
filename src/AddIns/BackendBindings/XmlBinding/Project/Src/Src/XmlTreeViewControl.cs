// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2741 $</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Displays a tree of XML elements. This is a separate control so it can
	/// be unit tested. It has no SharpDevelop specific parts, for example,
	/// the context menus are defined in the XmlTreeViewContainerControl.
	/// </summary>
	public class XmlTreeViewControl : ExtTreeView
	{			
		const string ViewStatePropertyName = "XmlTreeViewControl.ViewState";

		XmlDocument document;

		enum InsertionMode {
			Before = 0,
			After = 1
		}
		
		/// <summary>
		/// Raised when the delete key is pressed.
		/// </summary>
		public event EventHandler DeleteKeyPressed;
		
		public XmlTreeViewControl()
		{
		}
		
		/// <summary>
		/// Gets or sets the xml document currently being displayed.
		/// </summary>
		[Browsable(false)]
		public XmlDocument Document {
			get {
				return document;
			}
			set {
				document = value;
				
				// Update display.
				BeginUpdate();
				try {
					ShowDocument();
				} finally {
					EndUpdate();
				}
			}
		}
		
		/// <summary>
		/// Gets the selected element in the tree.
		/// </summary>
		public XmlElement SelectedElement {
			get {
				XmlElementTreeNode xmlElementTreeNode = SelectedElementNode;
				if (xmlElementTreeNode != null) {
					return xmlElementTreeNode.XmlElement;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Determines whether an element is selected in the tree.
		/// </summary>
		public bool IsElementSelected {
			get {
				return SelectedElement != null;
			}
		}
		
		/// <summary>
		/// Gets the selected text node in the tree.
		/// </summary>
		public XmlText SelectedTextNode {
			get {				
				XmlTextTreeNode xmlTextTreeNode = SelectedNode as XmlTextTreeNode;
				if (xmlTextTreeNode != null) {
					return xmlTextTreeNode.XmlText;
				}
				return null;
			}
		}
				
		/// <summary>
		/// Gets the selected comment node in the tree.
		/// </summary>
		public XmlComment SelectedComment {
			get {				
				XmlCommentTreeNode commentTreeNode = SelectedNode as XmlCommentTreeNode;
				if (commentTreeNode != null) {
					return commentTreeNode.XmlComment;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Determines whether a text node is selected in the tree.
		/// </summary>
		public bool IsTextNodeSelected {
			get {
				return SelectedTextNode != null;
			}
		}
		
		/// <summary>
		/// Saves the current state of the tree.
		/// </summary>
		public void SaveViewState(Properties properties)
		{
			properties.Set(ViewStatePropertyName, TreeViewHelper.GetViewStateString(this));
		}
		
		/// <summary>
		/// Restores the node state of the tree.
		/// </summary>
		public void RestoreViewState(Properties properties)
		{
			TreeViewHelper.ApplyViewStateString(properties.Get(ViewStatePropertyName, String.Empty), this);
		}
		
		/// <summary>
		/// Appends a new child element to the currently selected node.
		/// </summary>
		public void AppendChildElement(XmlElement element)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlElementTreeNode newNode = new XmlElementTreeNode(element);
				newNode.AddTo(selectedNode);
				selectedNode.Expand();
			}
		}
		
		/// <summary>
		/// Appends a new child text node to the currently selected element.
		/// </summary>
		public void AppendChildTextNode(XmlText textNode)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlTextTreeNode newNode = new XmlTextTreeNode(textNode);
				newNode.AddTo(selectedNode);
				selectedNode.Expand();
			}
		}
		
		/// <summary>
		/// Inserts a new element node before the currently selected
		/// node.
		/// </summary>
		public void InsertElementBefore(XmlElement element)
		{
			InsertElement(element, InsertionMode.Before);
		}
		
		/// <summary>
		/// Inserts a new element node after the currently selected
		/// node.
		/// </summary>
		public void InsertElementAfter(XmlElement element)
		{
			InsertElement(element, InsertionMode.After);
		}
		
		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		public void RemoveElement(XmlElement element)
		{
			XmlElementTreeNode node = FindElement(element);
			if (node != null) {
				node.Remove();
			}
		}
		
		/// <summary>
		/// Removes the specified text node from the tree.
		/// </summary>
		public void RemoveTextNode(XmlText textNode)
		{
			XmlTextTreeNode node = FindTextNode(textNode);
			if (node != null) {
				node.Remove();
			}
		}
		
		/// <summary>
		/// Inserts a text node before the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeBefore(XmlText textNode)
		{
			InsertTextNode(textNode, InsertionMode.Before);
		}
		
		/// <summary>
		/// Inserts a text node after the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeAfter(XmlText textNode)
		{
			InsertTextNode(textNode, InsertionMode.After);
		}
		
		/// <summary>
		/// Updates the corresponding tree node's text based on 
		/// the textNode's value.
		/// </summary>
		public void UpdateTextNode(XmlText textNode)
		{
			XmlTextTreeNode node = FindTextNode(textNode);
			if (node != null) {
				node.Update();
			}
		}
				
		/// <summary>
		/// Updates the corresponding tree node's text based on 
		/// the comment's value.
		/// </summary>
		public void UpdateComment(XmlComment comment)
		{
			XmlCommentTreeNode node = FindComment(comment);
			if (node != null) {
				node.Update();
			}
		}
		
		/// <summary>
		/// Appends a new child comment node to the currently selected element.
		/// </summary>
		public void AppendChildComment(XmlComment comment)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlCommentTreeNode newNode = new XmlCommentTreeNode(comment);
				newNode.AddTo(selectedNode);
				selectedNode.Expand();
			}
		}
		
		/// <summary>
		/// Removes the specified comment from the tree.
		/// </summary>
		public void RemoveComment(XmlComment comment)
		{
			XmlCommentTreeNode node = FindComment(comment);
			if (node != null) {
				node.Remove();
			}
		}
		
		/// <summary>
		/// Inserts a comment node before the currently selected
		/// node.
		/// </summary>
		public void InsertCommentBefore(XmlComment comment)
		{
			InsertComment(comment, InsertionMode.Before);
		}
		
		/// <summary>
		/// Inserts a comment node after the currently selected
		/// node.
		/// </summary>
		public void InsertCommentAfter(XmlComment comment)
		{
			InsertComment(comment, InsertionMode.After);
		}
		
		/// <summary>
		/// Updates the image so the corresponding tree node shows that
		/// it is in the process of being cut.
		/// </summary>
		public void ShowCut(XmlNode node)
		{
			ShowCut(node, true);
		}
		
		/// <summary>
		/// Updates the image so the corresponding tree node no longer
		/// shows it is in the process of being cut.
		/// </summary>
		public void HideCut(XmlNode node)
		{
			ShowCut(node, false);
		}
		
		/// <summary>
		/// If no node is selected after a mouse click then we make 
		/// sure the AfterSelect event is fired. Standard behaviour is
		/// for the AfterSelect event not to be fired when the user
		/// deselects all tree nodes.
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SelectedNode == null) {
				this.OnAfterSelect(new TreeViewEventArgs(null, TreeViewAction.ByMouse));
			}
		}
		
		/// <summary>
		/// Raises the DeleteKeyPressed event.
		/// </summary>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Delete && DeleteKeyPressed != null) {
				DeleteKeyPressed(this, new EventArgs());
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Displays the document in the xml tree.
		/// </summary>
		void ShowDocument()
		{
			Nodes.Clear();
			if (document != null) {
				foreach (XmlNode node in document.ChildNodes) {
					switch (node.NodeType) {
						case XmlNodeType.Element:
							XmlElementTreeNode elementNode = new XmlElementTreeNode((XmlElement)node);
							elementNode.AddTo(this);
							break;
						case XmlNodeType.Comment:
							XmlCommentTreeNode commentNode = new XmlCommentTreeNode((XmlComment)node);
							commentNode.AddTo(this);
							break;
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the selected xml element tree node.
		/// </summary>
		XmlElementTreeNode SelectedElementNode {
			get {
				return SelectedNode as XmlElementTreeNode;
			}
		}
		
		/// <summary>
		/// Inserts a new element node either before or after the 
		/// currently selected element node.
		/// </summary>
		void InsertElement(XmlElement element, InsertionMode insertionMode)
		{
			ExtTreeNode selectedNode = (ExtTreeNode)SelectedNode;
			if (selectedNode != null) {
				XmlElementTreeNode parentNode = (XmlElementTreeNode)selectedNode.Parent;
				XmlElementTreeNode newNode = new XmlElementTreeNode(element);
				int index = parentNode.Nodes.IndexOf(selectedNode);
				if (insertionMode == InsertionMode.After) {
					index++;
				}
				newNode.Insert(index, parentNode);
			}
		}
		
		/// <summary>
		/// Inserts a new text node either before or after the 
		/// currently selected node.
		/// </summary>
		void InsertTextNode(XmlText textNode, InsertionMode insertionMode)
		{
			ExtTreeNode selectedNode = (ExtTreeNode)SelectedNode;
			if (selectedNode != null) {
				XmlElementTreeNode parentNode = (XmlElementTreeNode)selectedNode.Parent;
				XmlTextTreeNode newNode = new XmlTextTreeNode(textNode);
				int index = parentNode.Nodes.IndexOf(selectedNode);
				if (insertionMode == InsertionMode.After) {
					index++;
				}
				newNode.Insert(index, parentNode);
			}
		}
		
		/// <summary>
		/// Inserts a new comment node either before or after the 
		/// currently selected node.
		/// </summary>
		void InsertComment(XmlComment comment, InsertionMode insertionMode)
		{
			ExtTreeNode selectedNode = (ExtTreeNode)SelectedNode;
			if (selectedNode != null) {
				ExtTreeNode parentNode = (ExtTreeNode)selectedNode.Parent;
				XmlCommentTreeNode newNode = new XmlCommentTreeNode(comment);
				int index = 0;
				if (parentNode != null) {
					index = parentNode.Nodes.IndexOf(selectedNode);
				} else {
					index = Nodes.IndexOf(selectedNode);
				}
				if (insertionMode == InsertionMode.After) {
					index++;
				}
				if (parentNode != null) {
					newNode.Insert(index, parentNode);
				} else {
					newNode.Insert(index, this);
				}
			}
		}
		
		/// <summary>
		/// Looks at all the nodes in the tree view and returns the
		/// tree node that represents the specified element.
		/// </summary>
		XmlElementTreeNode FindElement(XmlElement element, TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				XmlElementTreeNode elementTreeNode = node as XmlElementTreeNode;
				if (elementTreeNode != null) {
					if (elementTreeNode.XmlElement == element) {
						return elementTreeNode;
					}
					
					// Look for a match in the element's child nodes.
					XmlElementTreeNode childElementTreeNode = FindElement(element, elementTreeNode.Nodes);
					if (childElementTreeNode != null) {
						return childElementTreeNode;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the corresponding XmlElementTreeNode.
		/// </summary>
		XmlElementTreeNode FindElement(XmlElement element)
		{
			XmlElementTreeNode selectedElementTreeNode = SelectedNode as XmlElementTreeNode;
			if (selectedElementTreeNode != null && selectedElementTreeNode.XmlElement == element) {
				return selectedElementTreeNode;
			} else {
				return FindElement(element, Nodes);
			}
		}
		
		/// <summary>
		/// Looks at all the nodes in the tree view and returns the
		/// tree node that represents the specified text node.
		/// </summary>
		XmlTextTreeNode FindTextNode(XmlText textNode, TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				XmlTextTreeNode textTreeNode = node as XmlTextTreeNode;
				if (textTreeNode != null) {
					if (textTreeNode.XmlText == textNode) {
						return textTreeNode;
					}
				} else {
					// Look for a match in the node's child nodes.
					XmlTextTreeNode childTextTreeNode = FindTextNode(textNode, node.Nodes);
					if (childTextTreeNode != null) {
						return childTextTreeNode;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the specified text node in the tree.
		/// </summary>
		XmlTextTreeNode FindTextNode(XmlText textNode)
		{
			XmlTextTreeNode selectedTextTreeNode = SelectedNode as XmlTextTreeNode;
			if (selectedTextTreeNode != null && selectedTextTreeNode.XmlText == textNode) {
				return selectedTextTreeNode;
			} else {
				return FindTextNode(textNode, Nodes);
			}
		}
		
		/// <summary>
		/// Looks at all the nodes in the tree view and returns the
		/// tree node that represents the specified comment node.
		/// </summary>
		XmlCommentTreeNode FindComment(XmlComment comment, TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				XmlCommentTreeNode commentTreeNode = node as XmlCommentTreeNode;
				if (commentTreeNode != null) {
					if (commentTreeNode.XmlComment == comment) {
						return commentTreeNode;
					}
				} else {
					// Look for a match in the node's child nodes.
					XmlCommentTreeNode childCommentTreeNode = FindComment(comment, node.Nodes);
					if (childCommentTreeNode != null) {
						return childCommentTreeNode;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Locates the specified comment in the tree.
		/// </summary>
		XmlCommentTreeNode FindComment(XmlComment comment)
		{
			XmlCommentTreeNode selectedCommentTreeNode = SelectedNode as XmlCommentTreeNode;
			if (selectedCommentTreeNode != null && selectedCommentTreeNode.XmlComment == comment) {
				return selectedCommentTreeNode;
			} else {
				return FindComment(comment, Nodes);
			}
		}
		
		/// <summary>
		/// Shows the corresponding tree node with the ghosted image 
		/// that indicates it is being cut.
		/// </summary>
		void ShowCutElement(XmlElement element, bool showGhostImage)
		{
			XmlElementTreeNode node = FindElement(element);
			node.ShowGhostImage = showGhostImage;
		}
		
		/// <summary>
		/// Shows the corresponding tree node with the ghosted image 
		/// that indicates it is being cut.
		/// </summary>
		void ShowCutTextNode(XmlText textNode, bool showGhostImage)
		{
			XmlTextTreeNode node = FindTextNode(textNode);
			node.ShowGhostImage = showGhostImage;
		}
				
		/// <summary>
		/// Shows the corresponding tree node with the ghosted image 
		/// that indicates it is being cut.
		/// </summary>
		void ShowCutComment(XmlComment comment, bool showGhostImage)
		{
			XmlCommentTreeNode node = FindComment(comment);
			node.ShowGhostImage = showGhostImage;
		}
		
		/// <summary>
		/// Shows the cut node with a ghost image.
		/// </summary>
		/// <param name="showGhostImage">True if the node should be
		/// shown with the ghost image.</param>
		void ShowCut(XmlNode node, bool showGhostImage)
		{
			if (node is XmlElement) {
				ShowCutElement((XmlElement)node, showGhostImage);
			} else if (node is XmlText) {
				ShowCutTextNode((XmlText)node, showGhostImage);
			} else if (node is XmlComment) {
				ShowCutComment((XmlComment)node, showGhostImage);
			}
		}
	}
}
