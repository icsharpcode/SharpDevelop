// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
	public class RootNodeAddedToTreeControlTestFixture
	{
		XmlElementTreeNode rootNode;
		int nodeCount;
		XmlDocument doc;
		XmlElement initialElementSelected;
		bool initialIsElementSelected;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<test/>");
			using (XmlTreeViewControl treeView = new XmlTreeViewControl()) {
				treeView.DocumentElement = doc.DocumentElement;
				initialElementSelected = treeView.SelectedElement;
				initialIsElementSelected = treeView.IsElementSelected;
				
				// Set the document element again to make sure the existing node 
				// is removed.
				doc.LoadXml("<root/>");
				treeView.DocumentElement = null;
				treeView.DocumentElement = doc.DocumentElement;
				
				rootNode = (XmlElementTreeNode)treeView.Nodes[0];
				nodeCount = treeView.Nodes.Count;
			}
		}
		
		[Test]
		public void NoElementSelectedInitially()
		{
			Assert.IsNull(initialElementSelected);
		}
		
		[Test]
		public void IsNoElementSelectedInitially()
		{
			Assert.IsFalse(initialIsElementSelected);
		}
		
		[Test]
		public void OneNodeAdded()
		{
			Assert.AreEqual(1, nodeCount);
		}
			
		[Test]
		public void RootNodeText()
		{
			Assert.AreEqual("root", rootNode.Text);
		}
		
		[Test]
		public void ImageKey()
		{
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeImageKey, rootNode.ImageKey);
		}
		
		[Test]
		public void RootNodeHasNoChildren()
		{
			Assert.AreEqual(0, rootNode.Nodes.Count);
		}
		
		[Test]
		public void RootXmlElement()
		{
			Assert.IsTrue(Object.ReferenceEquals(rootNode.XmlElement, doc.DocumentElement));
		}
	}
}
