// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
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
