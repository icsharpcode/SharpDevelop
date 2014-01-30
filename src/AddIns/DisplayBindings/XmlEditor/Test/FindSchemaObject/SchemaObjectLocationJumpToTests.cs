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
