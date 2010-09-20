// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
