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

using NUnit.Framework;
using System;

namespace ICSharpCode.Core.Tests
{
	[TestFixture]
	public class ValidDirectoryNameTests
	{
		[Test]
		public void ValidDirectoryNameProjectTest()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("projecttest"));
		}
		
		[Test]
		public void ValidDirectoryNameWithPeriod()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("project.test"));
		}
		
		[Test]
		public void ValidDirectoryNameWithTwoPeriodAtStart()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("..projecttest"));
		}
		
		[Test]
		public void ValidDirectoryNameCOM()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("COM"));
		}
		
		[Test]
		public void ValidDirectoryNameCOM10()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("COM10"));
		}
		
		[Test]
		public void ValidDirectoryNameLPT()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("LPT"));
		}
		
		[Test]
		public void ValidDirectoryNameLPT10()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("LPT10"));
		}
		
		[Test]
		public void ValidEightThreeDirectoryName()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("projec~1.est"));
		}
		
		[Test]
		public void SmallestDirectoryNameLength()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName("a"));
		}
		
		[Test]
		public void LongestDirectoryNameLength()
		{
			// TODO: Determine what maximum length of directory should be.
			int MAX_LENGTH = 259;
			System.Text.StringBuilder tempString = new System.Text.StringBuilder("");
			for(int i=0; i < MAX_LENGTH; i++)
			{
				tempString.Append('a');
			}
			Assert.IsTrue(FileUtility.IsValidDirectoryEntryName(tempString.ToString()));
		}
	}
}
