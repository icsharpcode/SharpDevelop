// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2571 $</version>
// </file>

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
	}
}
