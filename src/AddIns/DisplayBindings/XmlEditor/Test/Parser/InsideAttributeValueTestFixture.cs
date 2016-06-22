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

namespace XmlEditor.Tests.Parser
{
	[TestFixture]
	public class InsideAttributeValueTestFixture
	{
		[Test]
		public void InvalidString()
		{
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(String.Empty, 10));
		}
		
		[Test]
		public void DoubleQuotesTest1()
		{
			string xml = "<foo a=\"";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void DoubleQuotesTest2()
		{
			string xml = "<foo a=\"\" ";
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void DoubleQuotesTest3()
		{
			string xml = "<foo a=\"\"";
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void DoubleQuotesTest4()
		{
			string xml = "<foo a=\" ";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void DoubleQuotesTest5()
		{
			string xml = "<foo a=\"\"";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, 8));
		}

		[Test]
		public void NoXmlElementStart()
		{
			string xml = "foo a=\"";
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void EqualsSignTest()
		{
			string xml = "<foo a=";
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void SingleQuoteTest1()
		{
			string xml = "<foo a='";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void MixedQuotesTest1()
		{
			string xml = "<foo a='\"";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void MixedQuotesTest2()
		{
			string xml = "<foo a=\"'";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void MixedQuotesTest3()
		{
			string xml = "<foo a=\"''";
			Assert.IsTrue(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}

		[Test]
		public void MixedQuotesTest4()
		{
			string xml = "<foo a=\"''\"";
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(xml, xml.Length));
		}
		
		[Test]
		public void NullString()
		{
			Assert.IsFalse(XmlParser.IsInsideAttributeValue(null, 0));
		}
	}
}
