// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class CreatePythonTestRunnerTestFixture
	{
		PythonTestFramework testFramework;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
		}
		
		[SetUp]
		public void Init()
		{
			testFramework = new PythonTestFramework();
		}
		
		[Test]
		public void PythonTestFrameworkCreateTestRunnerReturnsPythonTestRunner()
		{
			Assert.IsInstanceOf(typeof(PythonTestRunner), testFramework.CreateTestRunner());
		}
		
		[Test]
		public void PythonTestFrameworkCreateTestDebuggerReturnsPythonTestDebugger()
		{
			Assert.IsInstanceOf(typeof(PythonTestDebugger), testFramework.CreateTestDebugger());
		}
		
		[Test]
		public void PythonTestFrameworkIsBuildNeededBeforeTestRunReturnsFalse()
		{
			Assert.IsFalse(testFramework.IsBuildNeededBeforeTestRun);
		}
	}
}
