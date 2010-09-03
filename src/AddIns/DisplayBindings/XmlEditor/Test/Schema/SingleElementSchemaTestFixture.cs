// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
