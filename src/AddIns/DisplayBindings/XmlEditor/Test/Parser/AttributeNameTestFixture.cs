// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
