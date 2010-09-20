// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
