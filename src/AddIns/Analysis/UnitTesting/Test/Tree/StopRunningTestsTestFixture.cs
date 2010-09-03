// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class StopRunningTestsTestFixture : RunTestCommandTestFixtureBase
	{
		bool isUpdatedToolbarMethodCalledBeforeStopMethodCalled;
		MockBuildProjectBeforeTestRun buildProjectBeforeTestRun;
		MockTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			MockCSharpProject project = new MockCSharpProject();
			context.MockUnitTestsPad.AddProject(project);
			
			buildProjectBeforeTestRun = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProjectBeforeTestRun);
			
			testFramework = new MockTestFramework();
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project, testFramework);
			
			runTestCommand.Run();
			
			context.MockUnitTestsPad.IsUpdateToolbarMethodCalled = false;
			isUpdatedToolbarMethodCalledBeforeStopMethodCalled = context.MockUnitTestsPad.IsUpdateToolbarMethodCalled;
			runTestCommand.Stop();
		}
		
		[Test]
		public void IsOnStopMethodCalledReturnsTrue()
		{
			Assert.IsTrue(runTestCommand.IsOnStopMethodCalled);
		}
		
		[Test]
		public void IsRunningTestReturnsFalseAfterStopMethodCalled()
		{
			Assert.IsFalse(AbstractRunTestCommand.IsRunningTest);
		}
		
		[Test]
		public void RunningTestCommandPropertyReturnsNullAfterStopMethodCalled()
		{
			Assert.IsNull(AbstractRunTestCommand.RunningTestCommand);
		}
		
		[Test]
		public void IsUpdatedToolbarMethodCalledIsResetBeforeStopMethodCalled()
		{
			Assert.IsFalse(isUpdatedToolbarMethodCalledBeforeStopMethodCalled);
		}
		
		[Test]
		public void IsUpdatedToolbarMethodCalledAfterStopMethodCalled()
		{
			Assert.IsTrue(context.MockUnitTestsPad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void BuildCompleteEventFiringDoesNotCauseTestsToRunAfterStopMethodCalled()
		{
			buildProjectBeforeTestRun.FireBuildCompleteEvent();
			Assert.AreEqual(0, runTestCommand.TestRunnersCreated.Count);
		}
		
		[Test]
		public void ErrorListPadIsDisplayedWhenBuildCompleteEventIsFiredAfterStopMethodCalled()
		{
			FileName fileName = new FileName("test.cs");
			context.MockTaskService.Add(new Task(fileName, String.Empty, 1, 1, TaskType.Error));
			buildProjectBeforeTestRun.FireBuildCompleteEvent();
			Assert.IsTrue(context.MockUnitTestWorkbench.TypesPassedToGetPadMethod.Contains(typeof(ErrorListPad)));
		}
		
		[Test]
		public void StopMethodNotCalledAfterBuildCompleteEventFiresAfterStopMethodCalled()
		{
			context.MockTestResultsMonitor.IsStopMethodCalled = false;
			buildProjectBeforeTestRun.FireBuildCompleteEvent();
			Assert.IsFalse(context.MockTestResultsMonitor.IsStopMethodCalled);
		}
	}
}
