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
