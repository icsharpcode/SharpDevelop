// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
