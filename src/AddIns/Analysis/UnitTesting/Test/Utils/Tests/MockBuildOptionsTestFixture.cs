// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
