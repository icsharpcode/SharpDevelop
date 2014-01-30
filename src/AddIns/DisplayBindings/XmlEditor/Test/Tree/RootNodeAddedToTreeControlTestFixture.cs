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
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

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
		XmlNode documentElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DerivedXmlTreeViewContainerControl treeViewContainer = new DerivedXmlTreeViewContainerControl()) {
				treeViewContainer.LoadXml("<test/>");
				
				doc = treeViewContainer.Document;
				XmlTreeViewControl treeView = treeViewContainer.TreeView;
				
				treeViewContainer.Document = doc;
				initialElementSelected = treeView.SelectedElement;
				initialIsElementSelected = treeView.IsElementSelected;
				
				// Set the document element again to make sure the existing node 
				// is removed.
				doc.LoadXml("<root/>");
				treeViewContainer.Document = null;
				treeViewContainer.Document = doc;
				documentElement = treeViewContainer.Document.DocumentElement;
				
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
		
		[Test]
		public void DocumentElement()
		{
			Assert.AreSame(doc.DocumentElement, documentElement);
		}
	}
}
