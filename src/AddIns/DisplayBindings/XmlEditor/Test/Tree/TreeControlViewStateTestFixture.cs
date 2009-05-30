// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2741 $</version>
// </file>

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
