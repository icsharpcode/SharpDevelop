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
