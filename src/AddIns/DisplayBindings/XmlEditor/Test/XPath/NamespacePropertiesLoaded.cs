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
	public class NamespacePropertiesLoaded
	{
		List<XmlNamespace> expectedNamespaces;
		XmlNamespaceCollection actualNamespaces;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				Properties p = new Properties();
				expectedNamespaces = new List<XmlNamespace>();
				expectedNamespaces.Add(new XmlNamespace("f", "http://foo.com"));
				expectedNamespaces.Add(new XmlNamespace("s", "http://sharpdevelop.com"));
				
				List<string> namespaces = new List<string>();
				foreach (XmlNamespace xmlNamespace in expectedNamespaces) {
					namespaces.Add(xmlNamespace.ToString());
				}
				p.Set("Namespaces", namespaces.ToArray());
				queryControl.SetMemento(p);
				actualNamespaces = queryControl.GetNamespaces();
			}
		}
		
		[Test]
		public void TwoNamespacesAdded()
		{
			Assert.AreEqual(2, actualNamespaces.Count);
		}
		
		[Test]
		public void NamespacesAdded()
		{
			for (int i = 0; i < expectedNamespaces.Count; ++i) {
				XmlNamespace expectedNamespace = expectedNamespaces[i];
				XmlNamespace actualNamespace = actualNamespaces[i];
				Assert.AreEqual(expectedNamespace.Prefix, actualNamespace.Prefix);
				Assert.AreEqual(expectedNamespace.Name, actualNamespace.Name);
			}
		}
	}
}
