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
