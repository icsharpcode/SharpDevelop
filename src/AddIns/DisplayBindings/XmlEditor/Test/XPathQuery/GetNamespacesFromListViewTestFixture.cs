// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPathQuery
{
	/// <summary>
	/// Adds a set of namespaces to the XPathQueryControl and gets them back.
	/// </summary>
	[TestFixture]
	public class GetNamespacesFromListViewTestFixture
	{
		List<XmlNamespace> expectedNamespaces;
		ReadOnlyCollection<XmlNamespace> namespaces;
		List<XmlNamespace> namespacesAddedToGrid;
		
		[SetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				expectedNamespaces = new List<XmlNamespace>();
				expectedNamespaces.Add(new XmlNamespace("w", "http://www.wix.com"));
				expectedNamespaces.Add(new XmlNamespace("s", "http://sharpdevelop.com"));
				
				foreach (XmlNamespace ns in expectedNamespaces) {
					queryControl.AddNamespace(ns.Prefix, ns.Uri);
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
				Assert.AreEqual(expectedNamespace.Uri, actualNamespace.Uri);
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
				Assert.AreEqual(expectedNamespace.Uri, actualNamespace.Uri);
			}
		}
		
		[Test]
		public void TwoNamespacesAddedToGrid()
		{
			Assert.AreEqual(2, namespacesAddedToGrid.Count);
		}
	}
}
