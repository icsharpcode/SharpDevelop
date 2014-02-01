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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class AddElementsToTreeControlTestFixture : SDTestFixtureBase
	{
		XmlDocument doc;
		XmlElementTreeNode rootNode;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			using (DerivedXmlTreeViewContainerControl treeViewContainer = new DerivedXmlTreeViewContainerControl()) {
				treeViewContainer.LoadXml("<root/>");
				
				doc = treeViewContainer.Document;
				XmlTreeViewControl treeView = treeViewContainer.TreeView;
				
				rootNode = (XmlElementTreeNode)treeView.Nodes[0];

				// No node selected in treeview - adding a child 
				// node should do nothing.
				treeView.SelectedNode = null;
				XmlElement testElement = doc.CreateElement("test");
				treeViewContainer.AppendChildElement(testElement);
				
				treeView.SelectedNode = rootNode;
				XmlElement childElement = doc.CreateElement("child");
				treeViewContainer.AppendChildElement(childElement);
				
				// No node selected in treeview - inserting a node 
				// node should do nothing.
				treeView.SelectedNode = null;
				treeViewContainer.AppendChildElement(testElement);

				XmlElementTreeNode childNode = (XmlElementTreeNode)rootNode.Nodes[0];
				treeView.SelectedNode = childNode;
				XmlElement beforeElement = doc.CreateElement("before");
				treeViewContainer.InsertElementBefore(beforeElement);	
				
				// No node selected in treeview - inserting a node 
				// node should do nothing.
				treeView.SelectedNode = null;
				treeViewContainer.AppendChildElement(testElement);

				treeView.SelectedNode = childNode;
				XmlElement afterElement = doc.CreateElement("after");
				treeViewContainer.InsertElementAfter(afterElement);	
			}
		}
				
		[Test]
		public void ChildNodeAdded()
		{
			XmlElementTreeNode childNode = (XmlElementTreeNode)rootNode.Nodes[1];
			Assert.AreEqual("child", childNode.Text);
		}
				
		[Test]
		public void NodeInsertedBeforeAdded()
		{
			XmlElementTreeNode node = (XmlElementTreeNode)rootNode.Nodes[0];
			Assert.AreEqual("before", node.Text);
		}
		
		[Test]
		public void NodeInsertedBeforeParent()
		{
			XmlElementTreeNode node = (XmlElementTreeNode)rootNode.Nodes[0];
			Assert.IsNotNull(node.Parent);
		}

		[Test]
		public void NodeInsertedAfterAdded()
		{
			XmlElementTreeNode node = (XmlElementTreeNode)rootNode.Nodes[2];
			Assert.AreEqual("after", node.Text);
		}
		
		[Test]
		public void ThreeChildNodesAdded()
		{
			Assert.AreEqual(3, rootNode.Nodes.Count);
		}
	}
}
