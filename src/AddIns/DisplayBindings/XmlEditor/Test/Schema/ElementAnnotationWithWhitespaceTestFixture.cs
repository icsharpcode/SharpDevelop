// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class ElementAnnotationWithWhitespaceTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection fooChildElementCompletionItems;
		XmlCompletionItemCollection rootElementCompletionItems;
		
		public override void FixtureInit()
		{
			rootElementCompletionItems = SchemaCompletion.GetRootElementCompletion();
			
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("foo", "http://foo.com"));
			
			fooChildElementCompletionItems = SchemaCompletion.GetChildElementCompletion(path);
		}
		
		[Test]
		public void WhitespaceRemovedFromAnnotation()
		{
			string expectedText = 
				"First line\r\n" +
				"Second line\r\n" +
				"Third line\r\n" +
				"Fourth line";
			Assert.AreEqual(expectedText, rootElementCompletionItems[0].Description);
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://foo.com\" xmlns=\"http://foo.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:annotation>\r\n" +
				"\t\t\t<xs:documentation>\r\n" +
				"\t\t\tFirst line\r\n" +
				"\t\t\tSecond line\r\n" +
				"            Third line\r\n" +
				"            Fourth line\r\n" +
				"\t\t\t</xs:documentation>\r\n" +
				"\t\t</xs:annotation>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
