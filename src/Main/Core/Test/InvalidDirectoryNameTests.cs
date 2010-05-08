// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using NUnit.Framework;
using System;

namespace ICSharpCode.Core.Tests
{
	[TestFixture]
	public class InvalidDirectoryNameTests
	{
		[Test]
		public void ContainsBackslash()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(@"project\test"));
		}
		
		[Test]
		public void ContainsForwardSlash()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(@"project/test"));
		}
		
		[Test]
		public void IsPRN()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("PRN"));
		}
		
		[Test]
		public void IsEmptySpace()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(" "));
		}
		
		[Test]
		public void ContainsLessThan() 
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project<test"));
		}
		
		[Test]
		public void ContainsGreaterThan()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project>test"));
		}
		
		[Test]
		public void ContainsColon()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project:test"));
		}
		
		[Test]
		public void ContainsDoubleQuote()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(@"project""test"));
		}
		
		[Test]
		public void ContainsPipe()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project|test"));
		}
		
		[Test]
		public void ContainsQuestionMark()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project?test"));
		}
		
		[Test]
		public void ContainsStar()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("project*test"));
		}
		
		[Test]
		public void IntegerRepresentationBetween0And31()
		{
			for(int i=0; i<32; i++) 
			{
				char testChar = (char)i;
				string testString = string.Format("project{0}test", testChar);
				Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(testString));
			}
		}
		
		[Test]
		public void ReservedDeviceNameCON()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("CON"));
		}
		
		[Test]
		public void ReservedDeviceNamePRN()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("PRN"));
		}
		
		[Test]
		public void ReservedDeviceNameAUX()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("AUX"));
		}
		
		[Test]
		public void ReservedDeviceNameNUL()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("NUL"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM1()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM1"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM2()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM2"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM3()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM3"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM4()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM4"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM5()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM5"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM6()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM6"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM7()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM7"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM8()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM8"));
		}
		
		[Test]
		public void ReservedDeviceNameCOM9()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("COM9"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT1()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT1"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT2()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT2"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT3()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT3"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT4()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT4"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT5()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT5"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT6()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT6"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT7()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT7"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT8()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT8"));
		}
		
		[Test]
		public void ReservedDeviceNameLPT9()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("LPT9"));
		}
		
		[Test]
		public void EndWithSpace()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("projecttest "));
		}
		
		[Test]
		public void EndWithPeriod()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName("projecttest."));
		}

		/// <summary>
		/// Windows API defines the maximum length of a path to 260 characters.
		/// However, Unicode extended-length path may have a limit of approximately 32,767
		/// characters. See Maximum Path Length for more information at
		/// http://msdn.microsoft.com/en-us/library/aa365247(VS.85).aspx
		/// </summary>
		[Test]
		public void NameTooLong()
		{
			int MAX_LENGTH = 260;
			System.Text.StringBuilder tempString = new System.Text.StringBuilder("");
			for(int i=0; i < MAX_LENGTH; i++)
			{
				tempString.Append('a');
			}
			Assert.IsTrue(tempString.Length == MAX_LENGTH, "Failed to build a test directory string of {0} length.", MAX_LENGTH);
			Assert.IsFalse(FileUtility.IsValidDirectoryEntryName(tempString.ToString()));
		}
	}
}
