// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class SubstitutionGroupTests : SchemaTestFixtureBase
	{
		string namespaceURI = "http://www.w3.org/1999/XSL/Transform";
		
		protected override string GetSchema()
		{
			return
				"<xs:schema " +
				"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"" +
				"    xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"" +
				"    targetNamespace=\"http://www.w3.org/1999/XSL/Transform\"" +
				"    elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"stylesheet\" substitutionGroup=\"xsl:transform\"/>\r\n" +
				"</xs:schema>";
		}
		
		[Test]
		public void GetChildElementCompletion_ParentElementIsSubstitutionGroupButNoCorrespondingElementInSchema_NullReferenceExceptionIsNotThrown()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("stylesheet", namespaceURI));
			
			XmlCompletionItemCollection items = SchemaCompletion.GetChildElementCompletion(path);
			
			Assert.AreEqual(0, items.Count);
		}
	}
}
