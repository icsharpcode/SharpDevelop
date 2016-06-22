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
	/// Represents an xml comment in the tree.
	/// </summary>
	public class XmlCommentTreeNode : XmlCharacterDataTreeNode
	{
		public const string XmlCommentTreeNodeImageKey = "XmlCommentTreeNodeImage";
		public const string XmlCommentTreeNodeGhostImageKey = "XmlCommentTreeNodeGhostImage";

		XmlComment comment;
		
		public XmlCommentTreeNode(XmlComment comment)
			: base(comment)
		{
			this.comment = comment;
			ImageKey = XmlCommentTreeNodeImageKey;
			SelectedImageKey = ImageKey;
			Update();
		}
		
		/// <summary>
		/// Gets the XmlComment associated with this tree node.
		/// </summary>
		public XmlComment XmlComment {
			get {
				return comment;
			}
		}
		
		/// <summary>
		/// Gets or sets whether to show the ghost image which is 
		/// displayed when cutting the node.
		/// </summary>
		public bool ShowGhostImage {
			get {
				return ImageKey == XmlCommentTreeNodeGhostImageKey;
			}
			set {
				if (value) {
					ImageKey = XmlCommentTreeNodeGhostImageKey;
				} else {
					ImageKey = XmlCommentTreeNodeImageKey;
				}
				SelectedImageKey = ImageKey;
			}
		}
	}
}
