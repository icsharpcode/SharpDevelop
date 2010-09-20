// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Schema;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class SchemaObjectLocationJumpToTests
	{
		DerivedXmlSchemaObjectLocation location;
		
		[SetUp]
		public void Init()
		{
			XmlSchema schemaObject = new XmlSchema();
			schemaObject.SourceUri = @"d:\test\a.xsd";
			schemaObject.LineNumber = 2;
			schemaObject.LinePosition = 4;
			location = new DerivedXmlSchemaObjectLocation(schemaObject);
			location.JumpToFilePosition();
		}
		
		[Test]
		public void FileNameJumpedTo()
		{
			Assert.AreEqual(@"d:\test\a.xsd", location.JumpToFilePositionMethodFileNameParameter);
		}
		
		[Test]
		public void LineNumberJumpedTo()
		{
			Assert.AreEqual(2, location.JumpToFilePositionMethodLineParameter);
		}
		
		[Test]
		public void LinePositionJumpedTo()
		{
			Assert.AreEqual(4, location.JumpToFilePositionMethodColumnParameter);
		}
		
		[Test]
		public void JumpToDoesNothingWhenFileNameIsEmptyString()
		{
			XmlSchema schemaObject = new XmlSchema();
			schemaObject.SourceUri = String.Empty;
			DerivedXmlSchemaObjectLocation location = new DerivedXmlSchemaObjectLocation(schemaObject);
			Assert.IsFalse(location.IsDerivedJumpToFilePositionMethodCalled);
		}
	}
}
