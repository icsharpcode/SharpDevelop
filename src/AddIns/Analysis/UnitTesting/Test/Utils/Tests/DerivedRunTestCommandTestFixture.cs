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
			Assert.IsFalse(runTestCommand.IsRunningTestPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsTrueWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsTrue(runTestCommand.IsRunningTestPropertyWhenOnBeforeBuildCalled);
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
