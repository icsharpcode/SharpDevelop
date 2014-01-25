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
