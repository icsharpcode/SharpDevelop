// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class CreateRubyTestRunnerTestFixture
	{
		RubyTestFramework testFramework;
		
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
			testFramework = new RubyTestFramework();
		}
		
		[Test]
		public void RubyTestFrameworkCreateTestRunnerReturnsRubyTestRunner()
		{
			Assert.IsInstanceOf(typeof(RubyTestRunner), testFramework.CreateTestRunner());
		}
		
		[Test]
		public void RubyTestFrameworkCreateTestDebuggerReturnsRubyTestDebugger()
		{
			Assert.IsInstanceOf(typeof(RubyTestDebugger), testFramework.CreateTestDebugger());
		}
		
		[Test]
		public void RubyTestFrameworkIsBuildNeededBeforeTestRunReturnsFalse()
		{
			Assert.IsFalse(testFramework.IsBuildNeededBeforeTestRun);
		}
	}
}
