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
using ICSharpCode.SharpDevelop;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the XmlTreeViewContainerControl.OwnerState property.
	/// </summary>
	[TestFixture]
	public class OwnerStatusTestFixture : SDTestFixtureBase
	{
		DerivedXmlTreeViewContainerControl treeViewContainer;
		XmlTreeViewControl treeView;
		XmlDocument doc;
		XmlElementTreeNode htmlTreeNode;
		XmlElementTreeNode bodyTreeNode;
		XmlElementTreeNode paraTreeNode;
		XmlTextTreeNode textTreeNode;
		XmlCommentTreeNode commentTreeNode;
		
		[SetUp]
		public void Init()
		{
			treeViewContainer = new DerivedXmlTreeViewContainerControl();
			treeViewContainer.LoadXml("<!-- comment --><html><body class='a'><p>Text</p></body></html>");
			doc = treeViewContainer.Document;
			treeView = treeViewContainer.TreeView;
			
			commentTreeNode = (XmlCommentTreeNode)treeView.Nodes[0];
			htmlTreeNode = (XmlElementTreeNode)treeView.Nodes[1];
			htmlTreeNode.Expanding();
			
			bodyTreeNode = (XmlElementTreeNode)htmlTreeNode.Nodes[0];
			bodyTreeNode.Expanding();
			
			paraTreeNode = (XmlElementTreeNode)bodyTreeNode.Nodes[0];
			paraTreeNode.Expanding();
			
			textTreeNode = (XmlTextTreeNode)paraTreeNode.Nodes[0];
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainer != null) {
				treeViewContainer.Dispose();
			}
		}
		
		[Test]
		public void NothingSelected()
		{
			treeView.SelectedNode = null;
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.None, 
				treeViewContainer.InternalState,
				"OwnerState should be Nothing.");
		}
		
		[Test]
		public void RootElementSelected()
		{
			treeView.SelectedNode = htmlTreeNode;
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.RootElementSelected | XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.ElementSelected, 
				treeViewContainer.InternalState,
				"OwnerState should be RootElementSelected and ElementSelected.");
		}
		
		[Test]
		public void BodyElementSelected()
		{
			treeView.SelectedNode = bodyTreeNode;
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.ElementSelected, 
				treeViewContainer.InternalState,
				"OwnerState should be ElementSelected.");
		}
		
		[Test]
		public void ClassAttributeSelected()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.ShowAttributes(treeView.SelectedElement.Attributes);
			
			Assert.IsNotNull(treeViewContainer.AttributesGrid.SelectedGridItem,
				"Sanity check - should have a grid item selected.");
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.ElementSelected | XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.AttributeSelected, 
				treeViewContainer.InternalState,
				"OwnerState should be ElementSelected and AttributeSelected.");
		}
		
		[Test]
		public void TextNodeSelected()
		{
			treeView.SelectedNode = textTreeNode;
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.TextNodeSelected, 
				treeViewContainer.InternalState,
				"OwnerState should be TextNodeSelected.");
		}
		
		[Test]
		public void CommentNodeSelected()
		{
			treeView.SelectedNode = commentTreeNode;
			
			Assert.AreEqual(XmlTreeViewContainerControl.XmlTreeViewContainerControlStates.CommentSelected, 
				treeViewContainer.InternalState,
				"OwnerState should be CommentSelected.");
		}
	}
}
