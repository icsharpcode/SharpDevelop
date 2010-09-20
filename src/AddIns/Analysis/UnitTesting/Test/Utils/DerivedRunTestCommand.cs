// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class DerivedRunTestCommand : AbstractRunTestCommand
	{
		public bool IsOnBeforeBuildMethodCalled;
		public bool IsOnAfterRunTestsMethodCalled;
		public AbstractRunTestCommand RunningTestCommandPropertyWhenOnBeforeBuildCalled;
		public bool IsRunningTestWhenOnBeforeBuildCalled;
		public bool IsOnStopMethodCalled;
		public List<MockTestRunner> TestRunnersCreated = new List<MockTestRunner>();
		public int OnBeforeRunTestsMethodCallCount;
		
		public DerivedRunTestCommand(IRunTestCommandContext context)
			: base(context)
		{
		}

		public void CallOnBeforeBuildMethod()
		{
			OnBeforeBuild();
		}
		
		protected override void OnBeforeBuild()
		{
			IsOnBeforeBuildMethodCalled = true;
			RunningTestCommandPropertyWhenOnBeforeBuildCalled = AbstractRunTestCommand.RunningTestCommand;
			IsRunningTestWhenOnBeforeBuildCalled = AbstractRunTestCommand.IsRunningTest;
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			MockTestRunner testRunner = new MockTestRunner();
			TestRunnersCreated.Add(testRunner);
			return testRunner;
		}
		
		protected override void OnStop()
		{
			IsOnStopMethodCalled = true;
		}
		
		public Action<TestResult> ShowResultAction {
			get { return ShowResult; }
		}
		
		public void CallTestsCompleted()
		{
			base.TestRunCompleted();
		}

		public void CallOnAfterRunTestsMethod()
		{
			OnAfterRunTests();
		}
		
		protected override void OnAfterRunTests()
		{
			IsOnAfterRunTestsMethodCalled = true;
		}
		
		public ITestRunner CallCreateTestRunner(IProject project)
		{
			return CreateTestRunner(project);
		}
		
		protected override void OnBeforeRunTests()
		{
			OnBeforeRunTestsMethodCallCount++;
		}
	}
}
