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
	public class RunXPathQueryTests
	{		
		[Test]
		public void SingleEmptyElementNodeFoundByXPath()
		{
			string xml = 
				"<root>\r\n" +
				"\t<foo/>\r\n" +
				"</root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//foo");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "foo";
			string displayValue = "<foo/>";
			int lineNumber = 1;
			int linePosition = 2;
			XPathNodeType nodeType = XPathNodeType.Element;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
			Assert.AreEqual(1, nodes.Length);
		}
		
		[Test]
		public void XPathNodeMatchImplementsIXmlLineInfoInterface()
		{
			XPathNodeMatch node = new XPathNodeMatch(String.Empty, String.Empty, 1, 1, XPathNodeType.Text);
			Assert.IsNotNull(node as IXmlLineInfo);
		}
		
		[Test]
		public void OneElementNodeWithNamespaceFoundByXPath()
		{
			string xml = 
				"<root xmlns='http://foo.com'>\r\n" +
				"\t<foo></foo>\r\n" +
				"</root>";
			XmlNamespaceCollection namespaces = new XmlNamespaceCollection();
			namespaces.Add(new XmlNamespace("f", "http://foo.com"));
			XPathQuery query = new XPathQuery(xml, namespaces);
			XPathNodeMatch[] nodes = query.FindNodes("//f:foo");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "foo";
			string displayValue = "<foo>";
			int lineNumber = 1;
			int linePosition = 2;
			XPathNodeType nodeType = XPathNodeType.Element;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
			Assert.AreEqual(1, nodes.Length);
		}
		
		[Test]
		public void SingleElementWithNamespacePrefixFoundByXPath()
		{
			string xml = 
				"<f:root xmlns:f='http://foo.com'>\r\n" +
				"\t<f:foo></f:foo>\r\n" +
				"</f:root>";
			XmlNamespaceCollection namespaces = new XmlNamespaceCollection();
			namespaces.Add(new XmlNamespace("fo", "http://foo.com"));
			XPathQuery query = new XPathQuery(xml, namespaces);
			XPathNodeMatch[] nodes = query.FindNodes("//fo:foo");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "f:foo";
			string displayValue = "<f:foo>";
			int lineNumber = 1;
			int linePosition = 2;
			XPathNodeType nodeType = XPathNodeType.Element;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
			Assert.AreEqual(1, nodes.Length);
		}
		
		[Test]
		public void NoXPathNodeFoundForUnknownElementInXPathQuery()
		{
			string xml = 
				"<root>\r\n" +
				"\t<foo/>\r\n" +
				"</root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//bar");
			Assert.AreEqual(0, nodes.Length);
		}
		
		[Test]
		public void TextNodeMatchedWithXPath()
		{
			string xml = 
				"<root>\r\n" +
				"\t<foo>test</foo>\r\n" +
				"</root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//foo/text()");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "test";
			string displayValue = "test";
			int lineNumber = 1;
			int linePosition = 6;
			XPathNodeType nodeType = XPathNodeType.Text;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
			Assert.AreEqual(1, nodes.Length);
		}
		
		[Test]
		public void CommentNodeFoundByXPath()
		{
			string xml = "<!-- Test --><root/>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//comment()");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = " Test ";
			string displayValue = "<!-- Test -->";
			int lineNumber = 0;
			int linePosition = 4;
			XPathNodeType nodeType = XPathNodeType.Comment;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
		}
		
		[Test]
		public void EmptyCommentNodeFoundByXPath()
		{
			string xml = "<!----><root/>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//comment()");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = String.Empty;
			string displayValue = "<!---->";
			int lineNumber = 0;
			int linePosition = 4;
			XPathNodeType nodeType = XPathNodeType.Comment;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
			Assert.AreEqual(1, nodes.Length);
		}
		
		[Test]
		public void ProcessingInstructionNodeFoundByXPath()
		{
			string xml = "<root><?test processinstruction='1.0'?></root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//processing-instruction()");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "test";
			string displayValue = "<?test processinstruction='1.0'?>";
			int lineNumber = 0;
			int linePosition = 8;
			XPathNodeType nodeType = XPathNodeType.ProcessingInstruction;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
		}
		
		[Test]
		public void AttributeNodeFoundByXPath()
		{
			string xml = "<root>\r\n" +
				"\t<foo Id='ab'></foo>\r\n" +
				"</root>";
			XPathQuery query = new XPathQuery(xml);
			XPathNodeMatch[] nodes = query.FindNodes("//foo/@Id");
			XPathNodeMatch node = nodes[0];
			
			string nodeValue = "Id";
			string displayValue = "@Id";
			int lineNumber = 1;
			int linePosition = 6;
			XPathNodeType nodeType = XPathNodeType.Attribute;
			XPathNodeMatch expectedNode = new XPathNodeMatch(nodeValue, displayValue, lineNumber, linePosition, nodeType);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			Assert.IsTrue(comparison.AreEqual(expectedNode, node), comparison.GetReasonForNotMatching());
		}
	}
}
