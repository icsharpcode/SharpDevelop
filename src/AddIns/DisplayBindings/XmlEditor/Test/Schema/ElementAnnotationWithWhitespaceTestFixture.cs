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
