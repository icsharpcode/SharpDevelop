// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Schema;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class DerivedXmlSchemaObjectLocationTests
	{
		DerivedXmlSchemaObjectLocation location;
		
		[Test]
		public void DefaultIsDerivedJumpToFilePositionMethodCalledIsFalse()
		{
			DerivedXmlSchemaObjectLocation location = new DerivedXmlSchemaObjectLocation(null);
			Assert.IsFalse(location.IsDerivedJumpToFilePositionMethodCalled);
		}
		
		[Test]
		public void DefaultJumpToFilePositionMethodFileNameParameter()
		{
			DerivedXmlSchemaObjectLocation location = new DerivedXmlSchemaObjectLocation(null);
			Assert.IsNull(location.JumpToFilePositionMethodFileNameParameter);
		}
		
		[Test]
		public void DefaultJumpToFilePositionMethodLineParameter()
		{
			DerivedXmlSchemaObjectLocation location = new DerivedXmlSchemaObjectLocation(null);
			Assert.AreEqual(-1, location.JumpToFilePositionMethodLineParameter);
		}
		
		[Test]
		public void DefaultJumpToFilePositionMethodColumnParameter()
		{
			DerivedXmlSchemaObjectLocation location = new DerivedXmlSchemaObjectLocation(null);
			Assert.AreEqual(-1, location.JumpToFilePositionMethodColumnParameter);
		}
		
		[Test]
		public void JumpToFilePositionMethodCallIsRecorded()
		{
			int line = 2;
			int column = 3;
			location = new DerivedXmlSchemaObjectLocation(null);
			location.CallJumpToFilePosition("test.xml", line, column);
			
			Assert.IsTrue(location.IsDerivedJumpToFilePositionMethodCalled);
		}
		
		[Test]
		public void JumpToFilePositionMethodCallRecordsLineParameter()
		{
			JumpToFilePositionMethodCallIsRecorded();
			Assert.AreEqual(2, location.JumpToFilePositionMethodLineParameter);
		}
		
		[Test]
		public void JumpToFilePositionMethodCallRecordsColumnParameter()
		{
			JumpToFilePositionMethodCallIsRecorded();
			Assert.AreEqual(3, location.JumpToFilePositionMethodColumnParameter);
		}
		
		[Test]
		public void JumpToFilePositionMethodCallRecordsFileNameParameter()
		{
			JumpToFilePositionMethodCallIsRecorded();
			Assert.AreEqual("test.xml", location.JumpToFilePositionMethodFileNameParameter);
		}
	}
}
