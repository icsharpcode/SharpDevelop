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
			Assert.IsTrue(FileUtility.IsValidDirectoryName("projecttest"));
		}
		
		[Test]
		public void ValidDirectoryNameWithPeriod()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("project.test"));
		}
		
		[Test]
		public void ValidDirectoryNameWithTwoPeriodAtStart()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("..projecttest"));
		}
		
		[Test]
		public void ValidDirectoryNameCOM()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("COM"));
		}
		
		[Test]
		public void ValidDirectoryNameCOM10()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("COM10"));
		}
		
		[Test]
		public void ValidDirectoryNameLPT()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("LPT"));
		}
		
		[Test]
		public void ValidDirectoryNameLPT10()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("LPT10"));
		}
		
		[Test]
		public void ValidEightThreeDirectoryName()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("projec~1.est"));
		}
		
		[Test]
		public void SmallestDirectoryNameLength()
		{
			Assert.IsTrue(FileUtility.IsValidDirectoryName("a"));
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
			Assert.IsTrue(FileUtility.IsValidDirectoryName(tempString.ToString()));
		}
	}
}
