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
