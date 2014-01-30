// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
