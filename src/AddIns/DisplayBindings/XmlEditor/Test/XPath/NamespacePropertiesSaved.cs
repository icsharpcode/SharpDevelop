// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class NamespacePropertiesSaved
	{
		List<XmlNamespace> expectedNamespaces;
		string[] actualNamespaces;
		
		[SetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				expectedNamespaces = new List<XmlNamespace>();
				expectedNamespaces.Add(new XmlNamespace("f", "http://foo.com"));
				expectedNamespaces.Add(new XmlNamespace("s", "http://sharpdevelop.com"));
				
				foreach (XmlNamespace xmlNamespace in expectedNamespaces) {
					queryControl.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Name);
				}
				// Blank prefix and uris should be ignored.
				queryControl.AddNamespace(String.Empty, String.Empty);
				
				// Null cell values ignored.
				queryControl.NamespacesDataGridView.Rows.Add(new object[] {null, null});

				Properties p = queryControl.CreateMemento();
				actualNamespaces = p.Get("Namespaces", new string[0]);
			}
		}
		
		[Test]
		public void TwoNamespacesAdded()
		{
			Assert.AreEqual(2, actualNamespaces.Length);
		}
		
		[Test]
		public void NamespacesAdded()
		{
			for (int i = 0; i < expectedNamespaces.Count; ++i) {
				XmlNamespace expectedNamespace = expectedNamespaces[i];
				string actualNamespace = actualNamespaces[i];
				Assert.AreEqual(expectedNamespace.ToString(), actualNamespace);
			}
		}
	}
}
