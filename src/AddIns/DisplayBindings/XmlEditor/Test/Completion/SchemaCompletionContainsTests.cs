// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class SchemaCompletionContainsTests
	{
		XmlSchemaCompletionCollection schemas;

		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			schemas.Add(new XmlSchemaCompletion(new StringReader(xml)));
		}
		
		[Test]
		public void CollectionContainsTestSchemaNamespace()
		{
			Assert.IsTrue(schemas.Contains("http://test"));
		}
		
		[Test]
		public void CollectionDoesNotContainUnknownSchemaNamespace()
		{
			Assert.IsFalse(schemas.Contains("unknown-namespace"));
		}
	}
}
