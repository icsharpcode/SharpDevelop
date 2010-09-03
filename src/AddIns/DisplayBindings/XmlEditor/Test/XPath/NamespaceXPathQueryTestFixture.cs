// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class NamespaceXPathQueryTestFixture
	{
		XPathNodeMatch node;
		XPathNodeMatch xmlNamespaceNode;
		XPathNodeMatch[] nodes;
		
		[SetUp]
		public void Init()
		{
			string xml = "<root xmlns='http://foo.com'/>";
			XPathQuery query = new XPathQuery(xml);
			nodes = query.FindNodes("//namespace::*");
			node = nodes[0];
			xmlNamespaceNode = nodes[1];
		}
		
		[Test]
		public void TwoNamespaceNodesFoundByXPath()
		{
			Assert.AreEqual(2, nodes.Length);
		}
		
		[Test]
		public void FooNamespaceNodeFoundByXPath()
		{
			string nodeValue = "xmlns=\"http://foo.com\"";
			string displayValue = "xmlns=\"http://foo.com\"";
			int lineNumber = 0;
			int linePosition = 6;
			XPathNodeType nodeType = XPathNodeType.Namespace;
			XPathNodeMatch expectedMatch = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedMatch, node), comparison.GetReasonForNotMatching());
		}
		
		[Test]
		public void XmlNamespaceNodeFoundByXPathHasNoLineInfo()
		{
			Assert.IsFalse(xmlNamespaceNode.HasLineInfo());
		}
		
		[Test]
		public void XmlNamespaceNodeFoundByXPathHasW3OrgSchemaNamespace()
		{
			Assert.AreEqual("xmlns:xml=\"http://www.w3.org/XML/1998/namespace\"", xmlNamespaceNode.Value);
		}
	}
}
