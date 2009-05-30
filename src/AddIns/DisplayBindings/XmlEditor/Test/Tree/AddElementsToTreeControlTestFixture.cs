// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2111 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class AddElementsToTreeControlTestFixture
	{
		XmlDocument doc;
		XmlElementTreeNode rootNode;
		
		[SetUp]
		public void SetUpFixture()
		{
			using (XmlTreeViewContainerControl treeViewContainer = new XmlTreeViewContainerControl()) {
				XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(new XmlSchemaCompletionDataCollection(), null, String.Empty);
				treeViewContainer.LoadXml("<root/>", completionDataProvider);
				
				doc = treeViewContainer.Document;
				XmlTreeViewControl treeView = treeViewContainer.TreeView;
				
				//treeView.DocumentElement = doc.DocumentElement;
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
