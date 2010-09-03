// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
