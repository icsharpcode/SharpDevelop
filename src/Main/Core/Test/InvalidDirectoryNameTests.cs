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
			Assert.IsFalse(FileUtility.IsValidDirectoryName(@"project\test"));
		}
		
		[Test]
		public void ContainsForwardSlash()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryName(@"project/test"));
		}
		
		[Test]
		public void IsPRN()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryName("PRN"));
		}
		
		[Test]
		public void IsEmptySpace()
		{
			Assert.IsFalse(FileUtility.IsValidDirectoryName(" "));
		}
	}
}
