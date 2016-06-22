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
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Represents an XmlElement in the Xml Tree.
	/// </summary>
	public class XmlElementTreeNode : ExtTreeNode
	{
		public const string XmlElementTreeNodeImageKey = "XmlElementTreeNodeImage";
		public const string XmlElementTreeNodeGhostImageKey = "XmlElementTreeNodeGhostImage";
		
		XmlElement element;
		
		public XmlElementTreeNode(XmlElement element)
		{
			this.element = element;
			Text = GetDisplayText(element);
			Tag = element;
			ImageKey = XmlElementTreeNodeImageKey;
			
			if (element.HasChildNodes) {
				// Add dummy node so that the tree node can be
				// expanded in the tree view.
				Nodes.Add(new ExtTreeNode());
			}
		}
		
		/// <summary>
		/// Gets the XmlElement associated with this tree node.
		/// </summary>
		public XmlElement XmlElement {
			get {
				return element;
			}
		}
		
		/// <summary>
		/// Gets or sets whether to show the ghost image which is 
		/// displayed when cutting the node.
		/// </summary>
		public bool ShowGhostImage {
			get {
				return ImageKey == XmlElementTreeNodeGhostImageKey;
			}
			set {
				if (value) {
					ImageKey = XmlElementTreeNodeGhostImageKey;
				} else {
					ImageKey = XmlElementTreeNodeImageKey;
				}
				SelectedImageKey = ImageKey;
			}
		}
		
		/// <summary>
		/// Adds child elements to this tree node.
		/// </summary>
		protected override void Initialize()
		{
			Nodes.Clear();
			foreach (XmlNode childNode in element.ChildNodes) {
				XmlElement childElement = childNode as XmlElement;
				XmlText text = childNode as XmlText;
				XmlComment comment = childNode as XmlComment;
				if (childElement != null) {
					XmlElementTreeNode treeNode = new XmlElementTreeNode(childElement);
					treeNode.AddTo(this);
				} else if (text != null) {
					XmlTextTreeNode treeNode = new XmlTextTreeNode(text);
					treeNode.AddTo(this);
				} else if (comment != null) {
					XmlCommentTreeNode treeNode = new XmlCommentTreeNode(comment);
					treeNode.AddTo(this);
				}
			}
		}
		
		/// <summary>
		/// Gets the tree node's text for the element.
		/// </summary>
		static string GetDisplayText(XmlElement element)
		{
			if (element.Prefix.Length > 0) {
				return string.Concat(element.Prefix, ":", element.LocalName);
			}
			return element.LocalName;
		}
	}
}
