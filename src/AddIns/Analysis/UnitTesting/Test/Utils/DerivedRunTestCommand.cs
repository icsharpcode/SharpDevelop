// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
