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
	public class XmlElementTreeNode : ExtTreeNode
	{
		public const string XmlElementTreeNodeImageKey = "XmlElementTreeNodeImage";
		
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
		/// Adds child elements to this tree node.
		/// </summary>
		protected override void Initialize()
		{
			Nodes.Clear();
			foreach (XmlNode childNode in element.ChildNodes) {
				XmlElement childElement = childNode as XmlElement;
				XmlText text = childNode as XmlText;
				if (childElement != null) {
					XmlElementTreeNode treeNode = new XmlElementTreeNode(childElement);
					treeNode.AddTo(this);
				} else if (text != null) {
					XmlTextTreeNode treeNode = new XmlTextTreeNode(text);
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
				return String.Concat(element.Prefix, ":", element.LocalName);
			}
			return element.LocalName;
		}
	}
}
