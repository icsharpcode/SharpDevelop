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
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// The collection of schemas should provide completion data for the
	/// namespaces it holds.
	/// </summary>
	[TestFixture]
	public class NamespaceCompletionTestFixture
	{
		XmlCompletionItemCollection namespaceCompletionItems;
		string firstNamespace = "http://foo.com/foo.xsd";
		string secondNamespace = "http://bar.com/bar.xsd";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			XmlSchemaCompletionCollection items = new XmlSchemaCompletionCollection();
			
			StringReader reader = new StringReader(GetSchema(firstNamespace));
			XmlSchemaCompletion schema = new XmlSchemaCompletion(reader);
			items.Add(schema);
			
			reader = new StringReader(GetSchema(secondNamespace));
			schema = new XmlSchemaCompletion(reader);
			items.Add(schema);
			namespaceCompletionItems = items.GetNamespaceCompletion();
		}
		
		[Test]
		public void ShouldHaveTwoNamespaceCompletionItems()
		{
			Assert.AreEqual(2, namespaceCompletionItems.ToArray().Length);
		}
		
		[Test]
		public void NamespaceCompletionItemsCollectionContainsFirstNamespace()
		{
			Assert.IsTrue(namespaceCompletionItems.Contains(firstNamespace));
		}
		
		[Test]
		public void NamespaceCompletionItemsCollectionContainsSecondNamespace()
		{
			Assert.IsTrue(namespaceCompletionItems.Contains(secondNamespace));
		}		
		
		string GetSchema(string namespaceURI)
		{
			return "<?xml version=\"1.0\"?>\r\n" +
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"targetNamespace=\"" + namespaceURI + "\"\r\n" +
				"xmlns=\"" + namespaceURI + "\"\r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"<xs:element name=\"note\">\r\n" +
				"</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
