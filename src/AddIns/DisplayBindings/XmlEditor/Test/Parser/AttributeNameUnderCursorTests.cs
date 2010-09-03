// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
