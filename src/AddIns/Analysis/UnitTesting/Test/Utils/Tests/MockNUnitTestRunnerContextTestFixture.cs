// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockNUnitTestRunnerContextTestFixture
	{
		MockNUnitTestRunnerContext context;
		
		[SetUp]
		public void Init()
		{
			context = new MockNUnitTestRunnerContext();
		}
		
		[Test]
		public void MockProcessRunnerIsNotNull()
		{
			Assert.IsNotNull(context.MockProcessRunner);
		}
		
		[Test]
		public void MockTestResultsMonitorExists()
		{
			Assert.IsNotNull(context.MockTestResultsMonitor);
		}
		
		[Test]
		public void MockTestsResultsMonitorFileNameIsTmp66DotTmp()
		{
			string expectedFileName = @"c:\temp\tmp66.tmp";
			Assert.AreEqual(expectedFileName, context.MockTestResultsMonitor.FileName.ToString());
		}
	}
}
