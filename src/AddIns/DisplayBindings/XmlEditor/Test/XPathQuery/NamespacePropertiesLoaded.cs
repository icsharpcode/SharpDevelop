// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPathQuery
{
	[TestFixture]
	public class NamespacePropertiesLoaded
	{
		List<XmlNamespace> expectedNamespaces;
		ReadOnlyCollection<XmlNamespace> actualNamespaces;
		
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
				Assert.AreEqual(expectedNamespace.Uri, actualNamespace.Uri);
			}
		}
	}
}
