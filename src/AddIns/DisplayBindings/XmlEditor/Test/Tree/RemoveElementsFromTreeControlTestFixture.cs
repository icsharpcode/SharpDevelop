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
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class RemoveElementsFromTreeControlTestFixture
	{		
		XmlDocument doc;
		DerivedXmlTreeViewContainerControl treeViewContainerControl; 
		XmlTreeViewControl treeView;
		
		[SetUp]
		public void SetUp()
		{
			treeViewContainerControl = new DerivedXmlTreeViewContainerControl();
			treeView = treeViewContainerControl.TreeView;
			treeViewContainerControl.LoadXml("<root><child></child></root>");
			doc = treeViewContainerControl.Document;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainerControl != null) {
				treeViewContainerControl.Dispose();
			}
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsFalse(treeViewContainerControl.IsDirty);
		}
		
		[Test]
		public void RootTreeNodesBeforeRemove()
		{
			Assert.AreEqual(1, treeView.Nodes.Count);
		}
		
		[Test]
		public void RemoveSelectedRootElement()
		{
			treeView.SelectedNode = treeView.Nodes[0];
			treeView.RemoveElement(doc.DocumentElement);
			Assert.AreEqual(0, treeView.Nodes.Count);
		}
		
		[Test]
		public void RemoveRootElementWhenNoTreeNodeSelected()
		{
			treeView.SelectedNode = null;
			treeView.RemoveElement(doc.DocumentElement);
			Assert.AreEqual(0, treeView.Nodes.Count);	
		}
		
		[Test]
		public void RemoveChildElement()
		{
			ExtTreeNode rootNode = (ExtTreeNode)treeView.Nodes[0];
			rootNode.Expanding();
			treeView.RemoveElement((XmlElement)doc.DocumentElement.FirstChild);
			Assert.AreEqual(0, treeView.Nodes[0].Nodes.Count);
		}
		
		/// <summary>
		/// Removing an element that does not exist in the
		/// tree should not make the view dirty. Nothing should
		/// happen at all.
		/// </summary>
		[Test]
		public void RemoveUnknownElement()
		{
			XmlElement element = (XmlElement)doc.CreateElement("NewElement");
			treeView.RemoveElement(element);
		}
	}
}
