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
	/// <summary>
	/// Tests that we can detect the attribute's name.
	/// </summary>
	[TestFixture]
	public class AttributeNameTestFixture
	{		
		[Test]
		public void SuccessTest1()
		{
			string text = " foo='a";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}

		[Test]
		public void SuccessTest2()
		{
			string text = " foo='";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void SuccessTest3()
		{
			string text = " foo=";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest4()
		{
			string text = " foo=\"";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void SuccessTest5()
		{
			string text = " foo = \"";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest6()
		{
			string text = " foo = '#";
			QualifiedName expectedName = new QualifiedName("foo", String.Empty);
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name, "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void FailureTest1()
		{
			string text = "foo=";
			Assert.IsTrue(XmlParser.GetQualifiedAttributeName(text, text.Length).IsEmpty);
		}		
		
		[Test]
		public void FailureTest2()
		{
			string text = "foo=<";
			Assert.IsTrue(XmlParser.GetQualifiedAttributeName(text, text.Length).IsEmpty);
		}		
		
		[Test]
		public void FailureTest3()
		{
			string text = "a";
			Assert.IsTrue(XmlParser.GetQualifiedAttributeName(text, text.Length).IsEmpty);
		}	
		
		[Test]
		public void FailureTest4()
		{
			string text = " a";
			Assert.IsTrue(XmlParser.GetQualifiedAttributeName(text, text.Length).IsEmpty);
		}	
		
		[Test]
		public void EmptyString()
		{
			Assert.IsTrue(XmlParser.GetQualifiedAttributeName(String.Empty, 10).IsEmpty);
		}
		
		[Test]
		public void AttributeWithPrefix()
		{
			string text = " a:test=";
			QualifiedName expectedName = new QualifiedName("test", String.Empty, "a");
			QualifiedName name = XmlParser.GetQualifiedAttributeName(text, text.Length);
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void GetQualifiedAttributeNameWithEmptyString()
		{
			Assert.IsTrue(XmlParser.GetQualifiedAttributeNameAtIndex(String.Empty, 0, true).IsEmpty);
		}
		
		[Test]
		public void GetAttributeNameAtIndexWithNullString()
		{
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeNameAtIndex(null, 0));
		}
		
		[Test]
		public void GetAttributeNameWithNullString()
		{
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(null, 0));
		}
		
		[Test]
		public void GetQualifiedAttributeNameWithSingleXmlCharacter()
		{
			Assert.IsTrue(XmlParser.GetQualifiedAttributeNameAtIndex("<", 0, true).IsEmpty);
		}		
	}
}
