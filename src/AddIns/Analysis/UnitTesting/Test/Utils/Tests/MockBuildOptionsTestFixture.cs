// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockBuildOptionsTestFixture
	{
		MockBuildOptions buildOptions;
		
		[SetUp]
		public void Init()
		{
			buildOptions = new MockBuildOptions();
		}
		
		[Test]
		public void ShowErrorListAfterBuildReturnsTrueByDefault()
		{
			Assert.IsTrue(buildOptions.ShowErrorListAfterBuild);
		}
		
		[Test]
		public void ShowErrorListAfterBuildReturnsFalseWhenSetToFalse()
		{
			buildOptions.ShowErrorListAfterBuild = false;
			Assert.IsFalse(buildOptions.ShowErrorListAfterBuild);
		}
	}
}
