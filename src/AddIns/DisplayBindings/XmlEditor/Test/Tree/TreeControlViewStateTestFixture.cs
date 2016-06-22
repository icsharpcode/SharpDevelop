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
			string savedViewState = savedProperties.Get("XmlTreeViewControl.ViewState", string.Empty);
			Assert.AreEqual(expectedSavedViewState, savedViewState);
		}
		
		[Test]
		public void ViewStateLoaded()
		{
			Assert.AreEqual(String.Empty, restoredProperties.Get("XmlTreeViewControl.ViewState", string.Empty));
		}
	}
}
