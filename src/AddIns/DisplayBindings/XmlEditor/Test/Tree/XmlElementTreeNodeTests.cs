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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class XmlElementTreeNodeTextTests
	{
		XmlDocument doc;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<root/>");
		}
		
		[Test]
		public void UnqualifiedName()
		{
			XmlElement element = doc.CreateElement("child");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("child", node.Text);
		}

		[Test]
		public void QualifiedName()
		{
			XmlElement element = doc.CreateElement("child", "http://test.com");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("child", node.Text);
		}
		
		[Test]
		public void PrefixedName()
		{
			XmlElement element = doc.CreateElement("t", "child", "http://test.com");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("t:child", node.Text);
		}

	}
}
