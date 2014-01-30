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
