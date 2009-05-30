// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

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
