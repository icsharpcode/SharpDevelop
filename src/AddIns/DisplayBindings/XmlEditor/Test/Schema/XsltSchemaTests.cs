// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class XsltSchemaTests
	{
		string namespaceURI = "http://www.w3.org/1999/XSL/Transform";
		XmlSchemaCompletion schemaCompletion;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			schemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXsltSchema());
		}
		
		[Test]
		public void GetChildElementCompletion_StylesheetElement_SubstitutionGroupUsedForTemplateAndTemplateElementReturned()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("stylesheet", namespaceURI));
			
			XmlCompletionItemCollection completionItems = schemaCompletion.GetChildElementCompletion(path);
			bool contains = completionItems.Contains("template");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void GetAttributeCompletion_TemplateElementIsChildOfStylesheetElement_SubstitutionGroupUsedForTemplateAndMatchAttributeReturned()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("stylesheet", namespaceURI));
			path.AddElement(new QualifiedName("template", namespaceURI));
			
			XmlCompletionItemCollection completionItems = schemaCompletion.GetAttributeCompletion(path);
			bool contains = completionItems.Contains("match");
			
			Assert.IsTrue(contains);
		}
	}
}
