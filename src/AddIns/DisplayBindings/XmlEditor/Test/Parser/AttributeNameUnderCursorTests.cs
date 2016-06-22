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

namespace XmlEditor.Tests.Parser
{
	[TestFixture]
	public class AttributeNameUnderCursorTests
	{
		[Test]
		public void SuccessTest1()
		{
			string text = "<a foo";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest2()
		{
			string text = "<a foo";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.IndexOf("foo"));
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest3()
		{
			string text = "<a foo";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.IndexOf("oo"));
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest4()
		{
			string text = "<a foo";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length - 2);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest5()
		{
			string text = "<a foo=";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, 3);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest6()
		{
			string text = "<a foo=";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest7()
		{
			string text = "<a foo='";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest8()
		{
			string text = "<a type='a";
			QualifiedName expectedName = new QualifiedName("type", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SuccessTest9()
		{
			string text = "<a type='a'";
			QualifiedName expectedName = new QualifiedName("type", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length - 1);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void AttributeNameWithPrefix()
		{
			string text = "<a x:test=";
			QualifiedName expectedName = new QualifiedName("test", String.Empty, "x");
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void AttributeNameWithPrefix2()
		{
			string text = "<a xab:test=";
			QualifiedName expectedName = new QualifiedName("test", String.Empty, "xab");
			QualifiedName name = XmlParser.GetQualifiedAttributeNameAtIndex(text, text.IndexOf("xa"));
			Assert.AreEqual(expectedName, name);
		}
		

		[Test]
		public void GetAttributeNameInsideXmlElementText()
		{
			string xml = "<Test val1=\"\">as df</Test>";
			int offset = "<Test val1=\"\">as d".Length - 1;
			string result = XmlParser.GetAttributeNameAtIndex(xml, offset);
			Assert.AreEqual(String.Empty, result);
		}
	}
}
