// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
