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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPath
{
	/// <summary>
	/// Adds a set of namespaces to the XPathQueryControl and gets them back.
	/// </summary>
	[TestFixture]
	public class GetNamespacesFromListViewTestFixture
	{
		List<XmlNamespace> expectedNamespaces;
		XmlNamespaceCollection namespaces;
		List<XmlNamespace> namespacesAddedToGrid;
		
		[SetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				expectedNamespaces = new List<XmlNamespace>();
				expectedNamespaces.Add(new XmlNamespace("w", "http://www.wix.com"));
				expectedNamespaces.Add(new XmlNamespace("s", "http://sharpdevelop.com"));
				
				foreach (XmlNamespace ns in expectedNamespaces) {
					queryControl.AddNamespace(ns.Prefix, ns.Name);
				}
				
				namespacesAddedToGrid = new List<XmlNamespace>();
				for (int i = 0; i < queryControl.NamespacesDataGridView.Rows.Count - 1; ++i) {
					DataGridViewRow row = queryControl.NamespacesDataGridView.Rows[i];
					namespacesAddedToGrid.Add(new XmlNamespace((string)row.Cells[0].Value, (string)row.Cells[1].Value));
				}
				
				namespaces = queryControl.GetNamespaces();
			}
		}
		
		[Test]
		public void NamespacesMatch()
		{
			for (int i = 0; i < expectedNamespaces.Count; ++i) {
				XmlNamespace expectedNamespace = expectedNamespaces[i];
				XmlNamespace actualNamespace = namespaces[i];

				Assert.AreEqual(expectedNamespace.Prefix, actualNamespace.Prefix);
				Assert.AreEqual(expectedNamespace.Name, actualNamespace.Name);
			}
		}
		
		[Test]
		public void TwoNamespacesReturned()
		{
			Assert.AreEqual(2, namespaces.Count);
		}
		
		[Test]
		public void GridNamespacesMatch()
		{
			for (int i = 0; i < expectedNamespaces.Count; ++i) {
				XmlNamespace expectedNamespace = expectedNamespaces[i];
				XmlNamespace actualNamespace = namespacesAddedToGrid[i];

				Assert.AreEqual(expectedNamespace.Prefix, actualNamespace.Prefix);
				Assert.AreEqual(expectedNamespace.Name, actualNamespace.Name);
			}
		}
		
		[Test]
		public void TwoNamespacesAddedToGrid()
		{
			Assert.AreEqual(2, namespacesAddedToGrid.Count);
		}
	}
}
