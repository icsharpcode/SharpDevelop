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
	/// <summary>
	/// Retrieve completion data for an xml schema that specifies only one 
	/// element.
	/// </summary>
	[TestFixture]
	public class SingleElementSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection childElementCompletionItems;
		XmlCompletionItemCollection attributeCompletionItems;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("note", "http://www.w3schools.com"));

			attributeCompletionItems = SchemaCompletion.GetAttributeCompletion(path);
			childElementCompletionItems = SchemaCompletion.GetChildElementCompletion(path);
		}
		
		[Test]
		public void NamespaceUri()
		{
			Assert.AreEqual("http://www.w3schools.com", 
			                SchemaCompletion.NamespaceUri,
			                "Unexpected namespace.");
		}
		
		[Test]
		public void NoteElementHasNoAttributes()
		{
			Assert.AreEqual(0, attributeCompletionItems.Count, 
			                "Not expecting any attributes.");
		}
		
		[Test]
		public void NoteElementHasNoChildElements()
		{
			Assert.AreEqual(0, childElementCompletionItems.Count,
			                "Not expecting any child elements.");
		}
		
		protected override string GetSchema()
		{
			return "<?xml version=\"1.0\"?>\r\n" +
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"targetNamespace=\"http://www.w3schools.com\"\r\n" +
				"xmlns=\"http://www.w3schools.com\"\r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"<xs:element name=\"note\">\r\n" +
				"</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
