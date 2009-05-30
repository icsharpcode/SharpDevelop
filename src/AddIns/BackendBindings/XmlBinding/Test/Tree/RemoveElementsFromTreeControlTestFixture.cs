// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2111 $</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class RemoveElementsFromTreeControlTestFixture
	{		
		XmlDocument doc;
		XmlTreeViewContainerControl treeViewContainerControl; 
		XmlTreeViewControl treeView;
		
		[SetUp]
		public void SetUp()
		{
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(new XmlSchemaCompletionDataCollection(), null, String.Empty);
			treeViewContainerControl = new XmlTreeViewContainerControl();
			treeView = treeViewContainerControl.TreeView;
			treeViewContainerControl.LoadXml("<root><child></child></root>", completionDataProvider);
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
