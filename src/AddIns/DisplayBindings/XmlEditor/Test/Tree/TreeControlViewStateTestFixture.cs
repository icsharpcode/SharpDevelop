// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class TreeControlViewStateTestFixture
	{
		Properties savedProperties = new Properties();
		Properties restoredProperties = new Properties();
		string expectedSavedViewState;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<a><b>text</b><c>text</c></a>");
			
			// Save view state.
			using (XmlTreeViewControl treeView = new XmlTreeViewControl()) {
				treeView.Document = doc;
				ExtTreeNode node = (ExtTreeNode)treeView.Nodes[0];
				node.Expanding();
				expectedSavedViewState = TreeViewHelper.GetViewStateString(treeView);
				treeView.SaveViewState(savedProperties);
			}
			
			// Load view state.
			using (XmlTreeViewControl treeView = new XmlTreeViewControl()) {
				treeView.Document = doc;
				ExtTreeNode node = (ExtTreeNode)treeView.Nodes[0];
				node.Expanding();
				treeView.RestoreViewState(restoredProperties);				
			}
		}
		
		[Test]
		public void ViewStateSaved()
		{
			string savedViewState = (string)savedProperties.Get("XmlTreeViewControl.ViewState");
			Assert.AreEqual(expectedSavedViewState, savedViewState);
		}
		
		[Test]
		public void ViewStateLoaded()
		{
			Assert.AreEqual(String.Empty, restoredProperties.Get("XmlTreeViewControl.ViewState"));
		}
	}
}
