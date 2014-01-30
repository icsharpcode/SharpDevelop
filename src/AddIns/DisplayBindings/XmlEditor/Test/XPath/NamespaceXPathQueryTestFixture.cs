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
