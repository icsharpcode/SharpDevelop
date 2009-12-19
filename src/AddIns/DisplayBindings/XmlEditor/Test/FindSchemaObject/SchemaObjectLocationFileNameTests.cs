// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml.Schema;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class SchemaObjectLocationFileNameTests
	{
		[Test]
		public void FileColonTripleSlashUrlRemovedFromSchemaObjectFileName()
		{
			XmlSchema schemaObject = new XmlSchema();
			schemaObject.SourceUri = @"file:///d:\schemas\test.xsd";
			XmlSchemaObjectLocation location = new XmlSchemaObjectLocation(schemaObject);
			
			Assert.AreEqual(@"d:\schemas\test.xsd", location.FileName);
		}
		
		[Test]
		public void SchemaLocationFileNameIsEmptyStringIfSchemaObjectFileNameIsNull()
		{
			XmlSchema schemaObject = new XmlSchema();
			schemaObject.SourceUri = null;
			XmlSchemaObjectLocation location = new XmlSchemaObjectLocation(schemaObject);
			
			Assert.AreEqual(String.Empty, location.FileName);
		}
	}
}
