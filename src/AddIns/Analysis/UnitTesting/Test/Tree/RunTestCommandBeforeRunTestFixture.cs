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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestCommandBeforeRunTestFixture : RunTestCommandTestFixtureBase
	{
		MockCSharpProject project;
		MockBuildProjectBeforeTestRun buildProjectBeforeTestRun;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			project = new MockCSharpProject();
			context.MockUnitTestsPad.AddProject(project);
			
			buildProjectBeforeTestRun = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProjectBeforeTestRun);
			
			context.MockTaskService.BuildMessageViewCategory.SetText("Previous build text...");
			context.UnitTestCategory.SetText("Previous unit test run...");
		}
		
		[Test]
		public void RunningTestCommandPropertyIsNullByDefault()
		{
			Assert.IsNull(AbstractRunTestCommand.RunningTestCommand);
		}
		
		[Test]
		public void IsRunningTestReturnsFalseByDefault()
		{
			Assert.IsFalse(AbstractRunTestCommand.IsRunningTest);
		}
		
		[Test]
		public void RunCallsOnBeforeBuildMethod()
		{
			runTestCommand.Run();
			Assert.IsTrue(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void TaskServiceClearExceptCommentTasksMethodIsCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(context.MockTaskService.IsClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void TaskServiceIsInUpdateWhilstClearExceptCommentTasksMethodIsCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(context.MockTaskService.IsInUpdateWhilstClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void TaskServiceInUpdateReturnsFalseAfterRunMethodCompletes()
		{
			runTestCommand.Run();
			Assert.IsFalse(context.MockTaskService.InUpdate);
		}
		
		[Test]
		public void TaskServiceBuildMessageViewCategoryHasTextBeforeRunMethodCalled()
		{
			Assert.IsTrue(context.MockTaskService.BuildMessageViewCategory.Text.Length > 0);
		}
		
		[Test]
		public void TaskServiceBuildMessageViewCategoryTextIsClearedAfterRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.AreEqual(String.Empty, context.MockTaskService.BuildMessageViewCategory.Text);
		}
		
		[Test]
		public void TestRunnerMessageViewCategoryHasTextBeforeRunMethodCalled()
		{
			Assert.IsTrue(context.UnitTestCategory.Text.Length > 0);
		}
		
		[Test]
		public void TestRunnerMessageViewCategoryTextIsClearedAfterRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.AreEqual(String.Empty, context.UnitTestCategory.Text);
		}
		
		[Test]
		public void UnitTestsPadToolbarUpdatedAfterRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(context.MockUnitTestsPad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void UnitTestsPadBringToFrontMethodCalledAfterRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(context.MockUnitTestsPad.IsBringToFrontMethodCalled);
		}
		
		[Test]
		public void CompilerMessageViewPadDescriptorExistsInWorkbench()
		{
			Assert.IsNotNull(context.MockUnitTestWorkbench.GetPad(typeof(CompilerMessageView)));
		}
		
		[Test]
		public void CompilerMessageViewPadBroughtToFrontAfterRunMethodCalled()
		{
			runTestCommand.Run();
			Action expectedAction = context.MockUnitTestWorkbench.CompilerMessageViewPadDescriptor.BringPadToFront;
			Assert.AreEqual(expectedAction, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[0]);
		}
		
		[Test]
		public void RunningTestCommandPropertyIsSetToRunningCommandWhenOnBeforeRunIsCalled()
		{
			runTestCommand.Run();
			Assert.AreEqual(runTestCommand, runTestCommand.RunningTestCommandPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyReturnsTrueWhenOnBeforeRunIsCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(runTestCommand.IsRunningTestWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsUnitTestsPadResetTestResultsMethodCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(context.MockUnitTestsPad.IsResetTestResultsMethodCalled);
		}
		
		[Test]
		public void BuildProjectBeforeTestRunIsCreatedWhenRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.AreEqual(new[] { project }, buildProjectBeforeTestRun.Projects);
		}
		
		[Test]
		public void BuildProjectBeforeTestRunHasRunMethodCalledWhenRunTestCommandRunMethodCalled()
		{
			runTestCommand.Run();
			Assert.IsTrue(buildProjectBeforeTestRun.IsRunMethodCalled);
		}
	}
}
