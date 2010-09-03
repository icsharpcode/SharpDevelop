// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class CreateNUnitTestRunnerTestFixture
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
		}
		
		[Test]
		public void NUnitTestFrameworkCreateTestRunnerReturnsNUnitTestRunner()
		{
			NUnitTestFramework testFramework = new NUnitTestFramework();
			Assert.IsInstanceOf(typeof(NUnitTestRunner), testFramework.CreateTestRunner());
		}
		
		[Test]
		public void NUnitTestFrameworkCreateTestDebuggerReturnsNUnitTestDebugger()
		{
			NUnitTestFramework testFramework = new NUnitTestFramework();
			Assert.IsInstanceOf(typeof(NUnitTestDebugger), testFramework.CreateTestDebugger());
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsTrue()
		{
			NUnitTestFramework testFramework = new NUnitTestFramework();
			Assert.IsTrue(testFramework.IsBuildNeededBeforeTestRun);
		}
	}
}
