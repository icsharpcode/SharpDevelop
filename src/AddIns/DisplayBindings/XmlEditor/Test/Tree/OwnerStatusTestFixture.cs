// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the XmlTreeViewContainerControl.OwnerState property.
	/// </summary>
	[TestFixture]
	public class OwnerStatusTestFixture
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
