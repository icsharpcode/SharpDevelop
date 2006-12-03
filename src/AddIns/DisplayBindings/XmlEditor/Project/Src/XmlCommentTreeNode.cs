// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
	}
}
