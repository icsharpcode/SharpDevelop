// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class NoTestsRunWhenNoTestsSelectedTestFixture : RunTestCommandTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
			runTestCommand.Run();
		}
		
		[Test]
		public void OnBeforeRunIsNotCalled()
		{
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
	}
}
