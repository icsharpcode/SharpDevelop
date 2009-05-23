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
	/// Represents an XmlText node in the tree.
	/// </summary>
	public class XmlTextTreeNode : XmlCharacterDataTreeNode
	{
		public const string XmlTextTreeNodeImageKey = "XmlTextTreeNodeImage";
		public const string XmlTextTreeNodeGhostImageKey = "XmlTextTreeNodeGhostImage";
		
		XmlText xmlText;
		
		public XmlTextTreeNode(XmlText xmlText)
			: base(xmlText)
		{
			this.xmlText = xmlText;
			ImageKey = XmlTextTreeNodeImageKey;
			SelectedImageKey = ImageKey;
			Update();
		}
		
		/// <summary>
		/// Gets the XmlText associated with this tree node.
		/// </summary>
		public XmlText XmlText {
			get {
				return xmlText;
			}
		}
		
		/// <summary>
		/// Gets or sets whether to show the ghost image which is 
		/// displayed when cutting the node.
		/// </summary>
		public bool ShowGhostImage {
			get {
				return ImageKey == XmlTextTreeNodeGhostImageKey;
			}
			set {
				if (value) {
					ImageKey = XmlTextTreeNodeGhostImageKey;
				} else {
					ImageKey = XmlTextTreeNodeImageKey;
				}
				SelectedImageKey = ImageKey;
			}
		}
	}
}
