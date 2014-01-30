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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class BuildErrorWhenRunningTestsTestFixture : RunTestCommandTestFixtureBase
	{
		MockBuildProjectBeforeTestRun buildProjectBeforeTestRun;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			MockCSharpProject project = new MockCSharpProject();
			context.MockUnitTestsPad.AddProject(project);
			
			buildProjectBeforeTestRun = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProjectBeforeTestRun);
			
			runTestCommand.Run();
			
			FileName fileName = new FileName("test.cs");
			context.MockTaskService.Add(new Task(fileName, String.Empty, 1, 1, TaskType.Error));
			
			BuildError error = new BuildError();
			buildProjectBeforeTestRun.LastBuildResults.Add(error);
			buildProjectBeforeTestRun.FireBuildCompleteEvent();
		}
		
		[Test]
		public void NoTestRunnersCreated()
		{
			Assert.AreEqual(0, runTestCommand.TestRunnersCreated.Count);
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
	}
}
