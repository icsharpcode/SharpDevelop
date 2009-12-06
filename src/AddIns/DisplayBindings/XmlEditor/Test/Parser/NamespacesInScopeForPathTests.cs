// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Parser
{
	[TestFixture]
	public class NamespacesInScopeForPathTests
	{
		XmlNamespace xmlNamespace;
		
		[SetUp]
		public void Init()
		{
			 xmlNamespace = new XmlNamespace("xml", "http://www.w3.org/XML/1998/namespace");
		}
		
		[Test]
		public void XmlNamespaceInScopeForRootElementWithNoNamespace()
		{
			string xml = "<root ";
			XmlElementPath path = XmlParser.GetActiveElementStartPath(xml, xml.Length);
			
			XmlNamespace[] expectedNamespaces = new XmlNamespace[] {xmlNamespace};
			
			Assert.AreEqual(expectedNamespaces, path.NamespacesInScope.ToArray());
		}
		
		[Test]
		public void TwoNamespacesInScopeForRootElement()
		{
			string xml = "<root xmlns='test' ";
			XmlElementPath path = XmlParser.GetActiveElementStartPath(xml, xml.Length);
			
			XmlNamespace testNamespace = new XmlNamespace(String.Empty, "test");
			XmlNamespace[] expectedNamespaces = new XmlNamespace[] {xmlNamespace, testNamespace};
			
			Assert.AreEqual(expectedNamespaces, path.NamespacesInScope.ToArray());
		}
	}
}
