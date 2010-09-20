// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class DerivedRunTestCommandTestFixture
	{
		DerivedRunTestCommand runTestCommand;
		
		[SetUp]
		public void Init()
		{
			MockRunTestCommandContext context = new MockRunTestCommandContext();
			runTestCommand = new DerivedRunTestCommand(context);
		}
		
		[TearDown]
		public void TearDown()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
		}
		
		[Test]
		public void IsOnBeforeRunTestsMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void IsOnBeforeRunTestsMethodCalledReturnsTrueAfterOnBeforeRunMethodCalled()
		{
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsTrue(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsFalseByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsFalse(runTestCommand.IsRunningTestWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsTrueWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsTrue(runTestCommand.IsRunningTestWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNullByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsNull(runTestCommand.RunningTestCommandPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNonNullWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.AreEqual(runTestCommand, runTestCommand.RunningTestCommandPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsOnAfterRunTestsMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(runTestCommand.IsOnAfterRunTestsMethodCalled);
		}
		
		[Test]
		public void IsOnAfterRunTestsMethodCalledReturnsTrueAfterOnBeforeRunMethodCalled()
		{
			runTestCommand.CallOnAfterRunTestsMethod();
			Assert.IsTrue(runTestCommand.IsOnAfterRunTestsMethodCalled);
		}
		
		[Test]
		public void TestRunnersCreatedAreSaved()
		{
			MockTestRunner testRunner = runTestCommand.CallCreateTestRunner(null) as MockTestRunner;
			MockTestRunner[] expectedTestRunners = new MockTestRunner[] { testRunner };
			Assert.AreEqual(expectedTestRunners, runTestCommand.TestRunnersCreated.ToArray());
		}
	}
}
