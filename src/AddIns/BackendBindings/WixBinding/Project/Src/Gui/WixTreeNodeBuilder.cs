// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Creates and adds WixTreeNodes to a tree view.
	/// </summary>
	public class WixTreeNodeBuilder
	{
		WixTreeNodeBuilder()
		{
		}
		
		/// <summary>
		/// Adds a new tree node containing the xml element to the specified
		/// nodes collection.
		/// </summary>
		public static ExtTreeNode AddNode(ExtTreeNode parentNode, XmlElement element)
		{
			ExtTreeNode node = CreateNode(element);
			node.AddTo(parentNode);
			return node;
		}
		
		/// <summary>
		/// Adds a new tree node to the tree view.
		/// </summary>
		public static ExtTreeNode AddNode(ExtTreeView treeView, XmlElement element)
		{
			ExtTreeNode node = CreateNode(element);
			node.AddTo(treeView);
			return node;
		}
		
		/// <summary>
		/// Adds all the elements.
		/// </summary>
		public static void AddNodes(ExtTreeNode parentNode, XmlNodeList nodes)
		{
			foreach (XmlNode childNode in nodes) {
				XmlElement childElement = childNode as XmlElement;
				if (childElement != null) {
					AddNode(parentNode, childElement);
				}
			}
		}
		
		/// <summary>
		/// Creates a tree node from the specified element.
		/// </summary>
		static ExtTreeNode CreateNode(XmlElement element)
		{
			switch (element.LocalName) {
				case "Directory":
					return new WixDirectoryTreeNode((WixDirectoryElement)element);
				case "Component":
					return new WixComponentTreeNode((WixComponentElement)element);
				case "File":
					return new WixFileTreeNode((WixFileElement)element);
			}
			return new UnknownWixTreeNode(element);
		}
	}
}
