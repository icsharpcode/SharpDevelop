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
	public class NullSchemaObjectPassedToSchemaObjectLocationTestFixture
	{
		DerivedXmlSchemaObjectLocation location;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaObject schemaObject = null;
			location = new DerivedXmlSchemaObjectLocation(schemaObject);
			location.JumpToFilePosition();
		}
		
		[Test]
		public void FileNameIsEmptyString()
		{
			Assert.AreEqual(String.Empty, location.FileName);
		}
		
		[Test]
		public void LineNumberIsMinusOne()
		{
			Assert.AreEqual(-1, location.LineNumber);
		}
		
		[Test]
		public void LinePositionIsMinusOne()
		{
			Assert.AreEqual(-1, location.LinePosition);
		}
		
		[Test]
		public void JumpToFilePositionMethodDoesNothing()
		{
			location.JumpToFilePosition();
			Assert.IsFalse(location.IsDerivedJumpToFilePositionMethodCalled);
		}
	}
}
