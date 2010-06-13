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
			Assert.IsFalse(runTestCommand.IsOnBeforeRunTestsMethodCalled);
		}
		
		[Test]
		public void IsOnBeforeRunTestsMethodCalledReturnsTrueAfterOnBeforeRunMethodCalled()
		{
			runTestCommand.CallOnBeforeRunTestsMethod();
			Assert.IsTrue(runTestCommand.IsOnBeforeRunTestsMethodCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsFalseByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeRunTestsMethod();
			Assert.IsFalse(runTestCommand.IsRunningTestPropertyWhenOnBeforeRunCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsTrueWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeRunTestsMethod();
			Assert.IsTrue(runTestCommand.IsRunningTestPropertyWhenOnBeforeRunCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNullByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeRunTestsMethod();
			Assert.IsNull(runTestCommand.RunningTestCommandPropertyWhenOnBeforeRunCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNonNullWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeRunTestsMethod();
			Assert.AreEqual(runTestCommand, runTestCommand.RunningTestCommandPropertyWhenOnBeforeRunCalled);
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
