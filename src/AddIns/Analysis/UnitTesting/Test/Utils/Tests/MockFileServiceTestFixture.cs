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
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockFileServiceTestFixture
	{
		MockFileService fileService;
		
		[SetUp]
		public void Init()
		{
			fileService = new MockFileService();
		}
		
		[Test]
		public void FileOpenedReturnsNullByDefault()
		{
			Assert.IsNull(fileService.FileOpened);
		}
		
		[Test]
		public void FileOpenedReturnsFileNamePassedToOpenFileMethod()
		{
			fileService.OpenFile("test.cs");
			Assert.AreEqual("test.cs", fileService.FileOpened);
		}
		
		[Test]
		public void FilePositionsAreEqualWhenFileNameAndPositionAreEqual()
		{
			FilePosition lhs = new FilePosition("test.cs", 1, 2);
			FilePosition rhs = new FilePosition("test.cs", 1, 2);
			
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void FilePositionEqualsReturnsFalseWhenNullPassed()
		{
			FilePosition lhs = new FilePosition("test.cs", 1, 2);
			Assert.IsFalse(lhs.Equals(null));
		}
		
		[Test]
		public void FilePositionsAreNotEqualWhenFileNamesAreNotEqual()
		{
			FilePosition lhs = new FilePosition("test1.cs", 1, 2);
			FilePosition rhs = new FilePosition("test2.cs", 1, 2);
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void FilePositionsAreNotEqualWhenLinesAreNotEqual()
		{
			FilePosition lhs = new FilePosition("test.cs", 500, 2);
			FilePosition rhs = new FilePosition("test.cs", 1, 2);
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void FilePositionsAreNotEqualWhenColumnsAreNotEqual()
		{
			FilePosition lhs = new FilePosition("test.cs", 1, 2000);
			FilePosition rhs = new FilePosition("test.cs", 1, 2);
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void FilePositionJumpedToIsNullByDefault()
		{
			Assert.IsNull(fileService.FilePositionJumpedTo);
		}
		
		[Test]
		public void FilePositionJumpedToReturnsParametersPassedToJumpToFilePositionMethod()
		{
			int line = 1;
			int col = 10;
			string fileName = "test.cs";
			fileService.JumpToFilePosition(fileName, line, col);
			
			FilePosition expectedFilePos = new FilePosition(fileName, line, col);
			
			Assert.AreEqual(expectedFilePos, fileService.FilePositionJumpedTo);
		}
	}
}
