// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class NestedElementSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlElementPath noteElementPath;
		XmlCompletionItemCollection elementCompletionItems;
		
		public override void FixtureInit()
		{			
			noteElementPath = new XmlElementPath();
			noteElementPath.AddElement(new QualifiedName("note", "http://www.w3schools.com"));

			elementCompletionItems = SchemaCompletion.GetChildElementCompletion(noteElementPath); 
		}
		
		[Test]
		public void NoteHasOneChildElementCompletionDataItem()
		{
			Assert.AreEqual(1, elementCompletionItems.Count, "Should be one child element completion data item.");
		}
		
		[Test]
		public void NoteChildElementCompletionDataText()
		{
			Assert.IsTrue(elementCompletionItems.Contains("text"),
			              "Should be one child element called text.");
		}		

		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType> \r\n" +
				"\t\t\t<xs:sequence>\r\n" +
				"\t\t\t\t<xs:element name=\"text\" type=\"xs:string\"/>\r\n" +
				"\t\t\t</xs:sequence>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
